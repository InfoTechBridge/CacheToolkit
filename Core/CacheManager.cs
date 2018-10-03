using CacheToolkit.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Core
{
    public class CacheManager : ObjectCache
    {
        private readonly ObjectCache innerCache;
        private static CacheManager defaultInstance;

        private CacheManager()
            : this(CacheManagerConfiguration.Config)
        {

        }

        public CacheManager(ObjectCache cacheObject)
        {
            Contract.Requires<ArgumentNullException>(cacheObject != null);

            innerCache = cacheObject;
        }

        public CacheManager(CacheManagerConfiguration configuration)
        {
            Contract.Requires<ArgumentNullException>(configuration != null);

            CacheConfiguration conf = configuration.CacheConfigurations[configuration.DefaultCache];

            if (!string.IsNullOrEmpty(conf.StaticInstanceProperty.Trim()))
            { 
                //GetType(conf.Type).GetMethod("Run", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Invoke(null, null); // invoke static method
                innerCache = (ObjectCache)GetType(conf.Type).GetProperty(conf.StaticInstanceProperty, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).GetValue(null); // get static property
            }
            else
                innerCache = (string.IsNullOrEmpty(conf.Type.Trim()) ? MemoryCache.Default : (ObjectCache)System.Activator.CreateInstance(GetType(conf.Type)));
        }

        /// <summary>
        /// Gets a reference to the default instance.
        /// </summary>
        public static CacheManager Default
        {
            get
            {
                if (defaultInstance == null)
                    defaultInstance = new CacheManager();

                return defaultInstance;
            }
        }

        private static Type GetType(string typeName)
        {
            // If typeName is just FullName of the class It dos not returns type of classes that exist in another dll library
            // but if typeName is AssemblyQualifiedName it will be ok.
            // sampel: "System.Runtime.Caching.MemoryCache, System.Runtime.Caching, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
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

        #region ObjectCache implementation
        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            return innerCache.AddOrGetExisting(key, value, policy, regionName);
        }

        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            return innerCache.AddOrGetExisting(value, policy);
        }

        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return innerCache.AddOrGetExisting(key, value, absoluteExpiration, regionName);
        }

        public override bool Contains(string key, string regionName = null)
        {
            return innerCache.Contains(key, regionName);
        }

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            return innerCache.CreateCacheEntryChangeMonitor(keys, regionName);
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return innerCache.DefaultCacheCapabilities; }
        }

        public override object Get(string key, string regionName = null)
        {
            return innerCache.Get(key, regionName);
        }

        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            return innerCache.GetCacheItem(key, regionName);
        }

        public override long GetCount(string regionName = null)
        {
            return innerCache.GetCount(regionName);
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            //return (innerCache as IEnumerable<KeyValuePair<string, object>>).GetEnumerator();
            //return innerCache as IEnumerator<KeyValuePair<string, object>>;
            foreach (KeyValuePair<string, object> item in innerCache)
            {
                yield return item;
            }
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            return innerCache.GetValues(keys, regionName);
        }

        public override string Name
        {
            get { return innerCache.Name; }
        }

        public override object Remove(string key, string regionName = null)
        {
            return innerCache.Remove(key, regionName);
        }

        public override void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            innerCache.Set(key, value, policy, regionName);
        }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            innerCache.Set(item, policy);
        }

        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            innerCache.Set(key, value, absoluteExpiration, regionName);
        }

        public override object this[string key]
        {
            get
            {
                return innerCache[key];
            }
            set
            {
                innerCache[key] = value;
            }
        }
        #endregion ObjectCache implementation
    }
}
