using AnyCache.Core;
using AnyCache.InMemory;
using AnyCache.Redis;
using AnyCache.Serialization;
using AnyCache.Test.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AnyCache.Test.NetCore
{
    [TestClass]
    public class AnyCacheUnitTest
    {
        //IAnyCache cache = new InMemoryCache();

        //static ISerializer serializer = new BinarySerializer();
        static ISerializer serializer = new JsonSerializer();
        //static ISerializer serializer = new MsgPackSerializer();
        //static ISerializer serializer = new XmlSerializer();

        IAnyCache cache = new RedisCache(serializer: serializer);
        //static IAnyCache redis = new RedisCache();
        //IAnyCache cache = new CacheManager(redis);

        [TestMethod]
        public void TestValue()
        {
            string key = "key1";
            cache.Set(key, 123, null);

            int ret = (int)cache.Get(key);

            Assert.AreEqual(123, ret);

        }

        [TestMethod]
        public void TestValueTyped()
        {
            string key = "key1typed";
            cache.Set(key, 123000, null);

            int ret = cache.Get<int>(key);

            Assert.AreEqual(123000, ret);

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
        public void TestObjectTyped()
        {
            string key = "key1obj";
            Person obj = new Person("Tom", "Hanks");
            cache.Set(key, obj, null);

            Person ret = cache.Get<Person>(key);

            Assert.IsTrue(obj.PublicInstancePropertiesEqual(ret));

        }

        [TestMethod]
        public void TestListOfObjects()
        {
            string key = "key2list";
            List<Person> list = new List<Person>();
            list.Add(new Person("Tom", "Hanks"));
            list.Add(new Person("Tom1", "Hanks1"));
            list.Add(new Person("Tom2", "Hanks2"));
            list.Add(new Person("Tom3", "Hanks3"));

            cache.Set(key, list, null);

            //List<Person> ret = (List<Person>)cache.Get(key);
            List<Person> ret = cache.Get<List<Person>>(key);

            Assert.AreEqual(list.Count, ret.Count);
            Assert.IsTrue(list[0].PublicInstancePropertiesEqual(ret[0]));
            Assert.IsTrue(list[1].PublicInstancePropertiesEqual(ret[1]));
            Assert.IsTrue(list[2].PublicInstancePropertiesEqual(ret[2]));
            Assert.IsTrue(list[3].PublicInstancePropertiesEqual(ret[3]));
        }
    }
}
