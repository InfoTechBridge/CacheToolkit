using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnyCache.Core
{
    public interface IAnyCache : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        object this[string key] { get; set; }

        bool Add(string key, object value, DateTimeOffset? absoluteExpiration = null);
        bool Add<T>(string key, T value, DateTimeOffset? absoluteExpiration = null);
        bool Add(string key, object value, TimeSpan slidingExpiration);        
        bool Add<T>(string key, T value, TimeSpan slidingExpiration);

        object AddOrGetExisting(string key, object value, DateTimeOffset? absoluteExpiration = null);
        T AddOrGetExisting<T>(string key, T value, DateTimeOffset? absoluteExpiration = null);
        object AddOrGetExisting(string key, object value, TimeSpan slidingExpiration);
        T AddOrGetExisting<T>(string key, T value, TimeSpan slidingExpiration);

        void Set(string key, object value, DateTimeOffset? absoluteExpiration = null);
        void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration = null);
        void Set(string key, object value, TimeSpan slidingExpiration);
        void Set<T>(string key, T value, TimeSpan slidingExpiration);

        bool Contains(string key);

        object Get(string key);
        T Get<T>(string key);

        Task<object> GetAsync(string key);
        Task<T> GetAsync<T>(string key);

        IDictionary<string, object> GetAll(IEnumerable<string> keys);
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);

        object Remove(string key);
        T Remove<T>(string key);

        long GetCount();

        //IEnumerator<KeyValuePair<string, object>> GetEnumerator();

        
        void ClearCache();
        void Compact();
    }
}
