using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CacheToolkit.Serialization.Formatters
{
    /// <summary>
    /// Default constractor must be exist for objects. Dont need Seriazable attribute
    /// </summary>
    public class MsgPackFormatter : IFormatter
    {
         public ISurrogateSelector SurrogateSelector { get; set; }

        public SerializationBinder Binder { get; set; }

        public StreamingContext Context { get; set; }

        public MsgPackFormatter()
            : this(null, new StreamingContext(StreamingContextStates.All))
        {
           
        }

        public MsgPackFormatter(ISurrogateSelector selector, StreamingContext context)
        {
            SurrogateSelector = selector;
            Context = context;
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            using (BinaryWriter bw = new BinaryWriter(serializationStream))
            {
                bw.Write(graph.GetType().AssemblyQualifiedName);

                
                var serializer = MessagePackSerializer.Get(graph.GetType());
                serializer.Pack(serializationStream, graph);

                bw.Close();
            }
        }

        [SecuritySafeCritical]
        public object Deserialize(Stream serializationStream)
        {
            object retObject;
            using (BinaryReader br = new BinaryReader(serializationStream))
            {
                string className = br.ReadString();
                Type objectType = GetType(className);

                var serializer = MessagePackSerializer.Get(objectType);
                retObject = serializer.Unpack(serializationStream);

                br.Close();
            }
            return retObject;
        }

        private static Type GetType(string typeName)
        {
            // If typeName is just FullName of the class It dos not returns type of classes that exist in another dll library
            // but if typeName is AssemblyQualifiedName ("typeName,DllName" format) it will be ok.
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    break;
            }
            return type;
        }
    }
}
