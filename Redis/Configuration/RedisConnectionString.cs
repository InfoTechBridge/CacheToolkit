using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Redis.Configuration
{
    public class RedisConnectionString : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "DefaultConnection", IsKey = true, IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("connectionString", DefaultValue = "127.0.0.1:6379,abortConnect=false", IsKey = false, IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base["connectionString"]; }
            set { base["connectionString"] = value; }
        }

        [ConfigurationProperty("serializerType", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string SerializerType
        {
            get { return (string)base["serializerType"]; }
            set { base["serializerType"] = value; }
        }
    }
}
