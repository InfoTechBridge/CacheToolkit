using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CacheToolkit.Core.Configuration
{
    public class CacheConfiguration : ConfigurationElement
    {
        public CacheConfiguration(string name, string type, int port)
        {
            Name = name;
            Type = type;
        }

        public CacheConfiguration()
        {

        }

        [ConfigurationProperty("name", DefaultValue = "DefaultCache", IsKey = true, IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("type", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("staticInstanceProperty", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string StaticInstanceProperty
        {
            get { return (string)base["staticInstanceProperty"]; }
            set { base["staticInstanceProperty"] = value; }
        }
    }
}
