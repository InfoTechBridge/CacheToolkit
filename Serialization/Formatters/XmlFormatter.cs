using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CacheToolkit.Serialization.Formatters
{
    /// <summary>
    /// Default constractor must be exist for objects. Dont need Seriazable attribute
    /// </summary>
    public class XmlFormatter : IFormatter
    {
         public ISurrogateSelector SurrogateSelector { get; set; }

        public SerializationBinder Binder { get; set; }

        public StreamingContext Context { get; set; }

        public XmlFormatter()
            : this(null, new StreamingContext(StreamingContextStates.All))
        {
           
        }

        public XmlFormatter(ISurrogateSelector selector, StreamingContext context)
        {
            SurrogateSelector = selector;
            Context = context;
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            using (BinaryWriter bw = new BinaryWriter(serializationStream))
            {
                bw.Write(graph.GetType().AssemblyQualifiedName);

                XmlSerializer serializer = new XmlSerializer(graph.GetType());
                serializer.Serialize(serializationStream, graph);
                
                bw.Close();
            }
        }

        public object Deserialize(Stream serializationStream)
        {
            object retObject;
            using (BinaryReader br = new BinaryReader(serializationStream))
            {
                string className = br.ReadString();
                Type objectType = Type.GetType(className, true, true);

                XmlSerializer serializer = new XmlSerializer(objectType);
                retObject = serializer.Deserialize(serializationStream);
                
                br.Close();
            }
            return retObject;
        }

    }
}
