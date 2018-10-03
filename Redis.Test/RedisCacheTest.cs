using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using CacheToolkit.Redis.Configuration;

namespace CacheToolkit.Redis.Test
{
    [TestClass]
    public class RedisCacheTest
    {
        ObjectCache cache;

        public RedisCacheTest()
        {
            cache = Redis.RedisCache.Default;
        }

        [TestMethod]
        public void TestValueType()
        {
            string key = "key1";
            cache.Set(key, 123, null);

            int ret = (int)cache.Get(key);

            Assert.AreEqual(123, ret);

        }

        [TestMethod]
        public void TestObject()
        {
            string key = "key1obj";
            Person obj = new Person("Tom", "Hanks");
            cache.Set(key, obj, null);

            Person ret = (Person)cache.Get(key);

            Assert.IsTrue(obj.PublicInstancePropertiesEqual(ret));

        }

        [TestMethod]
        public void TestListObject()
        {
            string key = "key2list";
            List<Person> list = new List<Person>();
            list.Add(new Person("Tom", "Hanks"));
            list.Add(new Person("Tom1", "Hanks1"));
            list.Add(new Person("Tom2", "Hanks2"));
            list.Add(new Person("Tom3", "Hanks3"));

            cache.Set(key, list, null);

            List<Person> ret = (List<Person>)cache.Get(key);

            Assert.AreEqual(list.Count, ret.Count);
            Assert.IsTrue(list[0].PublicInstancePropertiesEqual(ret[0]));
            Assert.IsTrue(list[1].PublicInstancePropertiesEqual(ret[1]));
            Assert.IsTrue(list[2].PublicInstancePropertiesEqual(ret[2]));
            Assert.IsTrue(list[3].PublicInstancePropertiesEqual(ret[3]));
        }
    }
}
