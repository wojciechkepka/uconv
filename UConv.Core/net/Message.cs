using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace UConv.Core.Net
{
    [DataContract]
    public abstract class Message
    {
        public static T FromData<T>(string data)
            where T : Message
        {
            var ser = new DataContractSerializer(typeof(T), typeof(T).Name, "");
            var sr = new StringReader(data);
            var xr = new XmlTextReader(sr);
            return (T) ser.ReadObject(xr);
        }

        public string ToXmlString<T>()
            where T : Message
        {
            var ser = new DataContractSerializer(typeof(T), typeof(T).Name, "");
            var sw = new StringWriter();
            var xw = new XmlTextWriter(sw);
            ser.WriteObject(xw, this);
            return sw.ToString();
        }

        public byte[] ToXmlBinary<T>()
            where T : Message
        {
            return Encoding.ASCII.GetBytes(ToXmlString<T>());
        }
    }
}
