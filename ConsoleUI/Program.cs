using CacheToolkit.Core;
using CacheToolkit.Redis;
using CacheToolkit.Redis.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            //ConfigurationSectionManager defaultSection = new ConfigurationSectionManager(sectionName);
            //SettingsConfigurationCollection settingsCollection = new SettingsConfigurationCollection();
            //settingsCollection[0] = new SettingConfigurationElement() { Key = "Element", Value = "Element value" };
            //settingsCollection[1] = new SettingConfigurationElement() { Key = "NewElement", Value = "NeValueElement" };
            //settingsCollection[2] = new SettingConfigurationElement() { Key = "NewElement2", Value = "NeValueElement2" };
            //defaultSection.Settings = settingsCollection;
            //CreateConfigurationSection(sectionName, defaultSection);


            
            RedisConnectionString conn = RedisConfigurationSection.Config.RedisConnectionStrings[RedisConfigurationSection.Config.defaultConnection];
            ObjectCache cache = new Redis.RedisCache(conn.Name, conn.ConnectionString);

            ConfigurationSection section = RedisConfigurationSection.Config;
            Configuration config = ConfigurationManager.OpenExeConfiguration(null);
            config.Sections.Add("redisSettings", section);
            section.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Full);

            //ObjectCache cache = MemoryCache.Default;
            //ObjectCache cache = RedisCache.Default;
            //ObjectCache cache = new RedisCacheProvider.RedisCache("Binary", "localhost", new BinaryFormatter()); // needs Seriazable attribute
            //ObjectCache cache = new RedisCacheProvider.RedisCache("Soap", "localhost", new SoapFormatter()); // needs Seriazable attribute. does not support generic types
            //ObjectCache cache = new Redis.RedisCache("MsgPack", "localhost", new CacheToolkit.Serialization.Formatters.MsgPackFormatter()); 
            //ObjectCache cache = new RedisCacheProvider.RedisCache("Xml", "localhost", new RedisCacheProvider.Formatters.XmlFormatter()); 
            //ObjectCache cache = new RedisCacheProvider.RedisCache("Xml", "localhost", new RedisCacheProvider.Formatters.StringFormatter());
            

            //cache.Add("key1", (int)123, DateTimeOffset.MaxValue);
            cache.Set("key1", 123456789, null);
            Console.WriteLine(cache.Add("key1", (int)1234, null));
            var ret = cache.Get("key1");

            Console.WriteLine(ret);

            try
            {
                cache.Set("key2", DateTime.Now, null);

                ret = cache.Get("key2");
                Console.WriteLine(ret);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            cache.Set("key3", DateTime.Now.AddDays(10), null);

            ret = cache.Get("key3");
            Console.WriteLine(ret);

            try
            {
                Console.WriteLine(string.Format("Key count: {0}", cache.Count()));
                foreach (KeyValuePair<string, object> item in cache)
                    Console.WriteLine(string.Format("Key: {0} Value: {1}", item.Key, item.Value));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            cache.Remove("key1_STATE");

            CacheManager.Default.Set("testKey", "testValue", null, null);
            

            Console.ReadLine();
        }
    }
}
