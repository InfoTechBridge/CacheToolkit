using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Redis.Configuration
{
    [ConfigurationCollection(typeof(RedisConnectionString))]
    public class RedisConnectionStringCollection : ConfigurationElementCollection
    {
        //internal const string PropertyName = "connection";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        //protected override string ElementName
        //{
        //    get
        //    {
        //        return PropertyName;
        //    }
        //}

        //protected override bool IsElementName(string elementName)
        //{
        //    return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        //}

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RedisConnectionString();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RedisConnectionString)(element)).Name;
        }

        public RedisConnectionString this[int index]
        {
            get
            {
                return BaseGet(index) as RedisConnectionString;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new RedisConnectionString this[string name]
        {
            get { return (RedisConnectionString)BaseGet(name); }
            set
            {
                if (BaseGet(name) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(name)));
                }
                BaseAdd(value);
            }
        }

        public void Add(ConfigurationElement element)
        {
            BaseAdd(element);
        }
    }
}
