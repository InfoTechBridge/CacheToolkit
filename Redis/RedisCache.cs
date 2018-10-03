using CacheToolkit.Redis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Redis
{
    public class RedisCache : ObjectCache, IEnumerable, IDisposable
    {
        /// <summary>
        /// The ConnectionMultiplexer should attempt to reconnect automatically. So, should not need to dispose/reconnect.
        /// You can monitor the connection failure/reconnect via events published on the multiplexer instance.
        /// You can also use the .IsConnected() method on a database (this takes a key for server targeting reasons, but if you are only talking to one server, you could pass anything as the key).
        /// Marc Gravell
        /// </summary>
        ConnectionMultiplexer redisMultiplexer;
        IDatabase redisDatabase;
        IFormatter dataSerializer;

        private static RedisCache defaultCache;
        string instanceName;
        private readonly Object multiplexerConnectLock = new Object();
        //private bool isConnected;

        private RedisCache()
        {
            RedisConnectionString conn = RedisConfigurationSection.Config.RedisConnectionStrings[RedisConfigurationSection.Config.defaultConnection];

            Init("Default", conn.ConnectionString
                , string.IsNullOrEmpty(conn.SerializerType.Trim()) ? null : (IFormatter)System.Activator.CreateInstance(GetType(conn.SerializerType)));
            
            //Init("Default", "localhost,abortConnect=false");
        }

        public RedisCache(string name, string configuration, IFormatter serializer = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim() == "")
                throw new ArgumentException("The name can not be empty.", "name");

            if (name == "Default")
                throw new ArgumentException("The value \"Default\" cannot be assigned to a new RedisCache instance, because the value is reserved for use by the Default property.", "name");

            if (string.IsNullOrEmpty(configuration))
                throw new ArgumentException("configuration value can not be null or empty.", "configuration");

            Init(name, configuration, serializer);
        }

        private void Init(string name, string configuration, IFormatter serializer = null)
        {
            Connect(configuration);
            
            dataSerializer = serializer != null ? serializer : new BinaryFormatter();

            instanceName = name;
        }

        /// <summary>
        /// Gets a reference to the default RedisCache instance.
        /// </summary>
        public static RedisCache Default
        {
            get
            {
                if (defaultCache == null)
                    defaultCache = new RedisCache();

                return defaultCache;
            }
        }

        /// <summary>
        /// If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="policy"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            object ret = Get(key, regionName);
            if (ret == null)
                Set(key, value, policy.AbsoluteExpiration, regionName);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="policy"></param>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.</returns>
        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            CacheItem ret = GetCacheItem(value.Key, value.RegionName);
            if (ret == null)
                Set(value, policy);

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="regionName"></param>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.</returns>
        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            object ret = Get(key, regionName);
            if (ret == null)
                Set(key, value, absoluteExpiration, regionName);

            return ret;
        }

        /// <summary>
        /// Checks whether the cache entry already exists in the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public override bool Contains(string key, string regionName = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key value can not be null or empty.", "key");

            CheckAndReConnect();
            return redisDatabase.KeyExists(key);
        }

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a description of the features that the cache provides.
        /// Returns:
        //     A bitwise combination of flags that indicate the default capabilities of
        //     the cache implementation.
        /// </summary>
        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return DefaultCacheCapabilities.InMemoryProvider
                    | DefaultCacheCapabilities.OutOfProcessProvider
                    | DefaultCacheCapabilities.AbsoluteExpirations
                    | DefaultCacheCapabilities.CacheRegions;
            }
        }

        /// <summary>
        /// Gets the specified cache entry from the cache as an object.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.</returns>
        public override object Get(string key, string regionName = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key value can not be null or empty.", "key");

            CheckAndReConnect();
            RedisValue value = redisDatabase.StringGet(key);

            return value.IsNull ? null : RedisValueToObject(value);
        }

        /// <summary>
        /// Gets the specified cache entry from the cache as a CacheItem instance.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regionName"></param>
        /// <returns>If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.</returns>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            object ret = Get(key);
            if (ret == null)
                return null;

            else
                return new CacheItem(key, ret, regionName);
        }

        /// <summary>
        /// Returns the total number of cache entries in the cache.
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        public override long GetCount(string regionName = null)
        {
            CheckAndReConnect();
            int count = 0;
            System.Net.EndPoint[] endPoints = redisMultiplexer.GetEndPoints();
            foreach (System.Net.EndPoint endPoint in endPoints)
            {
                IServer server = redisMultiplexer.GetServer(endPoint);
                count += server.Keys(redisDatabase.Database, "*").Count(); // Send command "KEYS *" to server
            }
            return count;
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            CheckAndReConnect();
            System.Net.EndPoint[] endPoints = redisMultiplexer.GetEndPoints();
            foreach (System.Net.EndPoint endPoint in endPoints)
            {
                IServer server = redisMultiplexer.GetServer(endPoint);
                foreach (RedisKey key in server.Keys(redisDatabase.Database, "*")) // Send command "KEYS *" to server
                    yield return new KeyValuePair<string, object>(key, Get(key));
            }
        }

        /// <summary>
        /// Returns a set of cache entries that correspond to the specified keys.
        /// </summary>
        /// <param name="keys">A set of unique identifiers for the cache entries to return.</param>
        /// <param name="regionName"></param>
        /// <returns>A set of cache entries that correspond to the specified keys.</returns>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            RedisKey[] keysArray = new RedisKey[keys.Count()];
            int index = 0;
            foreach (string k in keys)
                keysArray[index++] = k;

            CheckAndReConnect();
            RedisValue[] values = redisDatabase.StringGet(keysArray);

            IDictionary<string, object> result = new Dictionary<string, object>();
            for (int i = 0; i < keysArray.Length; i++)
                result.Add(new KeyValuePair<string, object>(keysArray[i], values[i].IsNull ? null : RedisValueToObject(values[i])));

            return result;
        }

        public override string Name
        {
            get { return instanceName; }
        }

        /// <summary>
        /// Removes the cache entry from the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName"> Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>An object that represents the value of the removed cache entry that was specified by the key, or null if the specified entry was not found.</returns>
        public override object Remove(string key, string regionName = null)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key value can not be null or empty.", "key");

            object ret = Get(key, regionName);
            if (ret == null)
                return null;

            if (!redisDatabase.KeyDelete(key))
                throw new ApplicationException("Can not delete key from cache.");

            return ret;
        }

        /// <summary>
        /// If an item that matches key does not exist in the cache, value and key are used to insert as a new cache entry.
        /// If an item with a key that matches item exists, the cache entry is updated or overwritten by using value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="policy"></param>
        /// <param name="regionName"></param>
        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            Set(new CacheItem(key, value, regionName), policy);
        }

        /// <summary>
        /// If an item that matches key does not exist in the cache, value and key are used to insert as a new cache entry.
        /// If an item with a key that matches item exists, the cache entry is updated or overwritten by using value.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="policy"></param>
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.Key == null)
                throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(item.Key))
                throw new ArgumentException("Key can not be null or empty.", "item.Key");

            if (item.Value == null)
                throw new ArgumentNullException("item.Value");

            CheckAndReConnect();
            redisDatabase.StringSet(item.Key, ObjectToRedisValue(item.Value));
        }

        /// <summary>
        /// If an item that matches key does not exist in the cache, value and key are used to insert as a new cache entry.
        /// If an item with a key that matches item exists, the cache entry is updated or overwritten by using value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="regionName"></param>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            Set(new CacheItem(key, value, regionName), new CacheItemPolicy() { AbsoluteExpiration = absoluteExpiration });
        }

        public override object this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value, null);
            }
        }

        RedisValue ObjectToRedisValue(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                dataSerializer.Serialize(stream, obj);
                byte[] buffer = stream.ToArray();
                stream.Close();

                return buffer;
            }
        }

        object RedisValueToObject(RedisValue value)
        {
            using (MemoryStream stream = new MemoryStream(value))
            {
                object retObject = dataSerializer.Deserialize(stream);
                stream.Close();
                return retObject;
            }
        }
        
        private void Connect(string configuration)
        {
            redisMultiplexer = ConnectionMultiplexer.Connect(configuration);
            //isConnected = ((ConnectionMultiplexer)sender).IsConnected;
            //redisMultiplexer.ConnectionFailed += (sender, args) => isConnected = false;
            //redisMultiplexer.ConnectionRestored += (sender, args) => isConnected = ((ConnectionMultiplexer)sender).IsConnected;
            redisDatabase = redisMultiplexer.GetDatabase();
        }

        private void CheckAndReConnect()
        {
            if (!redisMultiplexer.IsConnected)// || !redisDatabase.IsConnected(default(RedisKey)))
                lock (multiplexerConnectLock)
                {
                    if (!redisMultiplexer.IsConnected)
                    {
                        string conf = redisMultiplexer.Configuration;
                        //redisMultiplexer.Dispose();
                        Connect(conf);
                    }
                }
        }

        public void Dispose()
        {
            redisMultiplexer.Dispose();
        }

        private static Type GetType(string typeName)
        {
            // If typeName is just FullName of the class It dos not returns type of classes that exist in another dll library
            // but if typeName is AssemblyQualifiedName ("typeName,DllName" format) it will be ok.
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    break;
            }
            return type;
        }
    }
}
