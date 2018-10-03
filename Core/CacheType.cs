using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Core
{
    public enum CacheType
    {
        [CacheInfo(typeof(System.Runtime.Caching.MemoryCache))]
        MemoryCache,

        [CacheInfo("CacheToolkit.Redis.RedisCache")]
        RedisCache,
    }
}
