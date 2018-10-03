using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;

namespace CacheToolkit.Redis.Test
{
    [TestClass]
    public class MemoryCacheTest
    {
        ObjectCache cache = MemoryCache.Default;

        [TestMethod]
        public void TestMemoryCache()
        {
            
            cache.Set("key1", (int)123, null);

            var ret = cache.Get("key1");

            Assert.AreEqual("123", ret.ToString());
        }
    }
}
