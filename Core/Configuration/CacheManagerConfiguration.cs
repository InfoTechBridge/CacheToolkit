using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Core.Configuration
{
    /// <summary>
    /// Use the following web.config file.
    ///<?xml version="1.0" encoding="utf-8" ?>
    ///<configuration>
    ///  <configSections>
    ///    <section name="cacheManager" type="CacheToolkit.Core.Configuration.CacheManagerConfiguration, CacheToolkit.Core" />
    ///  </configSections>
    ///  <cacheManager defaultCache="DefaultCache">
    ///     <caches>
    ///         <add name = "DefaultCache" [String] 
    ///          type = "" [String] />
    ///     </caches>
    ///  </cacheManager>
    ///</configuration>
    /// </summary>
    public class CacheManagerConfiguration : ConfigurationSection
    {
        public CacheManagerConfiguration()
        {
            CacheConfiguration item = new CacheConfiguration();
            CacheConfigurations.Add(item);

        }

        public static CacheManagerConfiguration Config
        {
            get
            {
                return (CacheManagerConfiguration)ConfigurationManager.GetSection("cacheManager") ?? CreateDefaultSection();
            }
        }

        [ConfigurationProperty("defaultCache", DefaultValue = "DefaultCache", IsRequired = true)]
        public string DefaultCache
        {
            get
            {
                return (string)this["defaultCache"];
            }
            set
            {
                this["defaultCache"] = value;
            }
        }

        [ConfigurationProperty("caches", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CacheConfigurationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public CacheConfigurationCollection CacheConfigurations
        {
            get { return ((CacheConfigurationCollection)(base["caches"])); }
            set { base["caches"] = value; }
        }

        private static CacheManagerConfiguration CreateDefaultSection()
        {
            CacheManagerConfiguration defaultSection = new CacheManagerConfiguration();
            defaultSection.CacheConfigurations = new CacheConfigurationCollection();
            defaultSection.CacheConfigurations.Add(new CacheConfiguration());
            return defaultSection;
        }
    }
}
