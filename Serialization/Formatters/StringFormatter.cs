using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CacheToolkit.Serialization.Formatters
{
    public class StringFormatter : IFormatter
    {
         public ISurrogateSelector SurrogateSelector { get; set; }

        public SerializationBinder Binder { get; set; }

        public StreamingContext Context { get; set; }

        public StringFormatter()
            : this(null, new StreamingContext(StreamingContextStates.All))
        {
           
        }

        public StringFormatter(ISurrogateSelector selector, StreamingContext context)
        {
            SurrogateSelector = selector;
            Context = context;
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            // Get fields that are to be serialized.
            MemberInfo[] members = FormatterServices.GetSerializableMembers(graph.GetType(), Context);

            // Get fields data.
            object[] objs = FormatterServices.GetObjectData(graph, members);

            // Write class name and all fields & values to stream
            using (StreamWriter sw = new StreamWriter(serializationStream))
            {
                sw.WriteLine(graph.GetType().AssemblyQualifiedName);
                for (int i = 0; i < objs.Length; ++i)
                {
                    sw.WriteLine("{0}={1}", members[i].Name, objs[i].ToString());
                }
                sw.Close();
            }
        }

        public object Deserialize(Stream serializationStream)
        {
            StreamReader sr = new StreamReader(serializationStream);
                                   
            string className = sr.ReadLine();
            Type t = Type.GetType(className);

            // Create object of just found type name.
            Object obj = FormatterServices.GetUninitializedObject(t);

            // Get type members.
            MemberInfo[] members = FormatterServices.GetSerializableMembers(obj.GetType(), Context);

            // Create data array for each member.
            object[] data = new object[members.Length];

            // Store serialized variable name -> value pairs.
            StringDictionary values = new StringDictionary();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] tokens = line.Split(new char[] { '=' });

                // key = variable name, value = variable value.
                values[tokens[0]] = line.Replace(tokens[0] + "=", string.Empty);
            }
            sr.Close();

            // Store for each member its value, converted from string to its type.
            for (int i = 0; i < members.Length; ++i)
            {
                FieldInfo fi = ((FieldInfo)members[i]);
                if (!values.ContainsKey(fi.Name))
                    throw new SerializationException("Missing field value : " + fi.Name);
                data[i] = System.Convert.ChangeType(values[fi.Name], fi.FieldType);
            }

            // Populate object members with theri values and return object.
            return FormatterServices.PopulateObjectMembers(obj, members, data);
        }
    }
}
