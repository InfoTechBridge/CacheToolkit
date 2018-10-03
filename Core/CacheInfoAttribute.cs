using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Core
{
    public class CacheInfoAttribute: Attribute
    {
        private Type type;
        private string typeName;
        
        public CacheInfoAttribute(Type type)
        {
            this.type = type;
            this.typeName = type.AssemblyQualifiedName;
        }

        public CacheInfoAttribute(string typeName)
        {
            this.typeName = typeName;
        }
        public Type Type
        {
            get
            {
                return this.type;
            }
        }
    }
}
