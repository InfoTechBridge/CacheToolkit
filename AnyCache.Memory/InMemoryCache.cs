#if NETFULL
using AnyCache.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace AnyCache.InMemory
{
    public class InMemoryCache : IAnyCache, IDisposable
    {
        private readonly ObjectCache _cache;

        public InMemoryCache()
        {
            _cache = MemoryCache.Default;
        }

        public InMemoryCache(ObjectCache cache)
        {
            _cache = cache;
        }

        public object this[string key] { get => Get(key); set => Set(key, value); }

        public bool Add(string key, object value, DateTimeOffset? absoluteExpiration = null)
        {
            return _cache.Add(key, value, absoluteExpiration.GetValueOrDefault(DateTimeOffset.MaxValue));
        }
        public bool Add<T>(string key, T value, DateTimeOffset? absoluteExpiration = null)
        {
            return _cache.Add(key, value, absoluteExpiration.GetValueOrDefault(DateTimeOffset.MaxValue));
        }
        public bool Add(string key, object value, TimeSpan slidingExpiration)
        {
            return _cache.Add(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }
        public bool Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            return _cache.Add(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }

        public object AddOrGetExisting(string key, object value, DateTimeOffset? absoluteExpiration = null)
        {
            return _cache.AddOrGetExisting(key, value, absoluteExpiration.GetValueOrDefault(DateTimeOffset.MaxValue));
        }
        public T AddOrGetExisting<T>(string key, T value, DateTimeOffset? absoluteExpiration = null)
        {
            return (T)_cache.AddOrGetExisting(key, value, absoluteExpiration.GetValueOrDefault(DateTimeOffset.MaxValue));
        }
        public object AddOrGetExisting(string key, object value, TimeSpan slidingExpiration)
        {
            return _cache.AddOrGetExisting(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }
        public T AddOrGetExisting<T>(string key, T value, TimeSpan slidingExpiration)
        {
            return (T)_cache.AddOrGetExisting(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }

        public void Set(string key, object value, DateTimeOffset? absoluteExpiration = null)
        {
            _cache.Set(key, value, absoluteExpiration.GetValueOrDefault(DateTimeOffset.MaxValue));
        }
        public void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration = null)
        {
            _cache.Set(key, value, absoluteExpiration.GetValueOrDefault(DateTimeOffset.MaxValue));
        }
        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            _cache.Set(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }
        public void Set<T>(string key, T value, TimeSpan slidingExpiration)
        {
            _cache.Set(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }

        public bool Contains(string key)
        {
            return _cache.Contains(key);
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }
        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public async Task<object> GetAsync(string key)
        {
            return await Task.Run(() =>
            {
                return _cache.Get(key);
            });
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.Run(() =>
            {
                return (T)_cache.Get(key);
            });
        }

        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            return _cache.GetValues(keys);
        }
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            return (IDictionary<string, T>)_cache.GetValues(keys).Select(i => new KeyValuePair<string, T>(i.Key, (T)i.Value));
        }

        public object Remove(string key)
        {
            return _cache.Remove(key);
        }
        public T Remove<T>(string key)
        {
            return (T)_cache.Remove(key);
        }

        public long GetCount()
        {
            return _cache.GetCount();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _cache.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void ClearCache()
        {
            throw new NotImplementedException();
        }

        public void Compact()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}

#endif
