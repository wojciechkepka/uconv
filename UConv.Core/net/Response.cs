using System.Runtime.Serialization;

namespace UConv.Core.Net
{
    [DataContract]
    public abstract class Response : Message
    {
        public Response(bool status)
        {
            this.status = status;
        }

        public bool status { get; set; }

        public new static T FromData<T>(string data)
            where T : Response
        {
            return Message.FromData<T>(data);
        }
    }
}
