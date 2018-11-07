using AnyCache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnyCache.Serialization
{
    public class XmlSerializer : ISerializer
    {
        public void Serialize(object value, Stream stream)
        {
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine(value.GetType().AssemblyQualifiedName);

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(value.GetType());
                serializer.Serialize(sw, value);

                sw.Close();
            }
        }

        public object Deserialize(Stream stream)
        {
            object retObject;
            using (StreamReader sr = new StreamReader(stream))
            {
                string className = sr.ReadLine();
                Type objectType = GetType(className);

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(objectType);
                retObject = serializer.Deserialize(sr);

                sr.Close();
            }
            return retObject;
        }

        public T Deserialize<T>(Stream stream)
        {
            T retObject;
            using (StreamReader sr = new StreamReader(stream))
            {
                string className = sr.ReadLine();
                //Type objectType = GetType(className);

                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                retObject = (T)serializer.Deserialize(sr);

                sr.Close();
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
