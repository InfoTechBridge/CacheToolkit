using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Redis.Test
{
    [Serializable]
    public class Person
    {
        public Person()
        {

        }

        public Person(string name, string family)
        {
            Name = name;
            Family = family;
        }
        public string Name { get; set; }
        public string Family { get; set; }
    }
}
