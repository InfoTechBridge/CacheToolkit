using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Redis.Configuration
{
    /// <summary>
    /// Use the following web.config file.
    ///<?xml version="1.0" encoding="utf-8" ?>
    ///<configuration>
    ///  <configSections>
    ///    <section name="redisConnectionStrings" type="CacheToolkit.Redis.Configuration.RedisCacheConfiguration, CacheToolkit.Redis" />
    ///  </configSections>
    ///  <redisSettings defaultConnection = "DefaultConnection">
    ///    <add name = "DefaultConnection" [String]
    ///          connectionString = "127.0.0.1:6379,abortConnect=false" [String]
    ///          serializerName = "" [String] />
    ///  </redisSettings>
    ///</configuration>
    /// </summary>
    public class RedisConfigurationSection : ConfigurationSection
    {
        public static RedisConfigurationSection Config
        {
            get 
            {
                return (RedisConfigurationSection)ConfigurationManager.GetSection("redisSettings") ?? CreateDefaultSection();
            }
        }

        [ConfigurationProperty("defaultConnection", DefaultValue = "DefaultConnection", IsRequired = true)]
        public string defaultConnection
        {
            get
            {
                return (string)this["defaultConnection"];
            }
            set
            {
                this["defaultConnection"] = value;
            }
        }

        [ConfigurationProperty("redisConnectionStrings", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(RedisConnectionStringCollection), AddItemName = "Redis")]
        public RedisConnectionStringCollection RedisConnectionStrings
        {
            get { return ((RedisConnectionStringCollection)(base["redisConnectionStrings"])); }
            set { base["redisConnectionStrings"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public void Save()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(null);
            RedisConfigurationSection instance = (RedisConfigurationSection)config.Sections["redisSettings"];
            instance.RedisConnectionStrings = this.RedisConnectionStrings;
            config.Save(ConfigurationSaveMode.Full);
        }

        private static RedisConfigurationSection CreateDefaultSection()
        {
            RedisConfigurationSection defaultSection = new RedisConfigurationSection();
            defaultSection.RedisConnectionStrings = new RedisConnectionStringCollection();
            defaultSection.RedisConnectionStrings.Add(new RedisConnectionString());
            return defaultSection;
        }
    }

    

    
}
