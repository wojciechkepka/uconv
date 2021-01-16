using System.Runtime.Serialization;

namespace UConv.Core.Net
{
    [DataContract]
    public abstract class Request : Message
    {
        public Request(Method method)
        {
            this.method = method;
        }

        [DataMember] public Method method { get; set; }

        public new static T FromData<T>(string data)
            where T : Request
        {
            return Message.FromData<T>(data);
        }
    }
}
