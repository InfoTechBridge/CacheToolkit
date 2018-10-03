using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Core
{
    public class CacheFactory
    {
        private Dictionary<CacheType, Func<ObjectCache>> products = new Dictionary<CacheType, Func<ObjectCache>>()
        {
            { CacheType.MemoryCache, () => MemoryCache.Default },
            { CacheType.RedisCache, () => new MemoryCache("Default1") }
        };
        
        public ObjectCache Create(CacheType type)
        {
            //var factory = products[description];
            //return factory();

            return products[type]();
            //return Activator.CreateInstance(t) as IMachine;
        }

        //private void LoadTypesICanReturn()
        //{
        //    products = new Dictionary<string, Type>();

        //    Type[] typesInThisAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

        //    foreach (Type type in typesInThisAssembly)
        //    {
        //        if (type.GetInterface(typeof(ObjectCache).ToString()) != null)
        //        {
        //            products.Add(type.Name.ToLower(), type);
        //        }
        //    }
        //}

        
    }
}
