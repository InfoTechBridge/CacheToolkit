using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AnyCache.Core
{
    public class CacheManager : IAnyCache
    {
        public CacheManager(IAnyCache cache)
        {
            innerCache = cache;
        }

        private readonly IAnyCache innerCache;

        public object this[string key]
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

        public bool Add(string key, object value, DateTimeOffset? absoluteExpiration = null)
        {
            return innerCache.Add(key, value, absoluteExpiration);
        }
        public bool Add<T>(string key, T value, DateTimeOffset? absoluteExpiration = null)
        {
            return innerCache.Add<T>(key, value, absoluteExpiration);
        }
        public bool Add(string key, object value, TimeSpan slidingExpiration)
        {
            return innerCache.Add(key, value, slidingExpiration);
        }
        public bool Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            return innerCache.Add<T>(key, value, slidingExpiration);
        }

        public object AddOrGetExisting(string key, object value, DateTimeOffset? absoluteExpiration = null)
        {
            return innerCache.AddOrGetExisting(key, value, absoluteExpiration);
        }
        public T AddOrGetExisting<T>(string key, T value, DateTimeOffset? absoluteExpiration = null)
        {
            return innerCache.AddOrGetExisting<T>(key, value, absoluteExpiration);
        }
        public object AddOrGetExisting(string key, object value, TimeSpan slidingExpiration)
        {
            return innerCache.AddOrGetExisting(key, value, slidingExpiration);
        }
        public T AddOrGetExisting<T>(string key, T value, TimeSpan slidingExpiration)
        {
            return innerCache.AddOrGetExisting<T>(key, value, slidingExpiration);
        }

        public void Set(string key, object value, DateTimeOffset? absoluteExpiration = null)
        {
            innerCache.Set(key, value, absoluteExpiration);
        }
        public void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration = null)
        {
            innerCache.Set<T>(key, value, absoluteExpiration);
        }
        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            innerCache.Set(key, value, slidingExpiration);
        }
        public void Set<T>(string key, T value, TimeSpan slidingExpiration)
        {
            innerCache.Set<T>(key, value, slidingExpiration);
        }

        public bool Contains(string key)
        {
            return innerCache.Contains(key);
        }

        public object Get(string key)
        {
            return innerCache.Get(key);
        }
        public T Get<T>(string key)
        {
            return innerCache.Get<T>(key);
        }

        public Task<object> GetAsync(string key)
        {
            return innerCache.GetAsync(key);
        }

        public Task<T> GetAsync<T>(string key)
        {
            return innerCache.GetAsync<T>(key);
        }

        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            return innerCache.GetAll(keys);
        }
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            return innerCache.GetAll<T>(keys);
        }

        public object Remove(string key)
        {
            return innerCache.Remove(key);
        }
        public T Remove<T>(string key)
        {
            return innerCache.Remove<T>(key);
        }

        public long GetCount()
        {
            return innerCache.GetCount();
        }
               
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return innerCache.GetEnumerator();
        }
        
        public void ClearCache()
        {
            innerCache.ClearCache();
        }

        public void Compact()
        {
            innerCache.Compact();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerCache.GetEnumerator();
        }
    }
}
