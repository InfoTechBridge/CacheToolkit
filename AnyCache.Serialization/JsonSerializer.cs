using AnyCache.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;

namespace AnyCache.Serialization
{
    public class JsonSerializer : ISerializer
    {
        private readonly Encoding encoding;

        public JsonSerializer(Encoding encoding = null)
        {
            if (encoding != null)
                this.encoding = encoding;
            else
                this.encoding = Encoding.Default;
        }

        //public string Serialize(object value)
        //{
        //    return JsonConvert.SerializeObject(value);
        //}

        //public object Deserialize(string value)
        //{
        //    return JsonConvert.DeserializeObject(value);
        //}

        //public T Deserialize<T>(string value)
        //{
        //    return JsonConvert.DeserializeObject<T>(value);
        //}

        public void Serialize(object value, Stream stream)
        {
            using (StreamWriter sw = new StreamWriter(stream, encoding))
            {
                var x = JsonConvert.SerializeObject(value);
                sw.Write(x);
            }
        }

        public object Deserialize(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream, encoding))
            {
                string jsonString = sr.ReadToEnd();
                //dynamic r0 = JValue.Parse(jsonString);
                dynamic r = JsonConvert.DeserializeObject(jsonString);
                return r;
            }
        }

        public T Deserialize<T>(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream, encoding))
            {
                return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            }
        }
    }
}
