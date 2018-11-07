using AnyCache.Core;
using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AnyCache.Serialization
{
    public class MsgPackSerializer : ISerializer
    {
        public void Serialize(object value, Stream stream)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                //bw.Write(value.GetType().AssemblyQualifiedName);

                var serializer = MessagePackSerializer.Get(value.GetType());
                serializer.Pack(stream, value);

                bw.Close();
            }
        }

        public object Deserialize(Stream stream)
        {
            object retObject;
            using (BinaryReader br = new BinaryReader(stream))
            {
                string className = br.ReadString();
                Type objectType = GetType(className);

                var serializer = MessagePackSerializer.Get(objectType);
                retObject = serializer.Unpack(stream);

                br.Close();
            }
            return retObject;
        }

        public T Deserialize<T>(Stream stream)
        {
            T retObject;
            using (BinaryReader br = new BinaryReader(stream))
            {
                //string className = br.ReadString();
                ////Type objectType = GetType(className);

                var serializer = MessagePackSerializer.Get(typeof(T));
                retObject = (T)serializer.Unpack(stream);

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
