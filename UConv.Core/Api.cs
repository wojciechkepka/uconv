using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using static UConv.Core.Units;

namespace UConv.Core
{
    public enum Method
    {
        ConverterList,
        Convert,
        ExchangeRate
    }

    [DataContract]
    public abstract class Message
    {

        public static T FromData<T>(string data)
        where T : Message
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(T), typeof(T).Name.ToString(), "");
            var sr = new StringReader(data);
            var xr = new XmlTextReader(sr);
            return (T)ser.ReadObject(xr);
        }

        public string ToXmlString<T>()
        where T : Message
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(T), typeof(T).Name.ToString(), "");
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

    [DataContract]
    public abstract class Request : Message
    {
        [DataMember]
        public Method method { get; set; }

        public Request(Method method)
        {
            this.method = method;
        }

        public new static T FromData<T>(string data)
        where T : Request
        {
            return Message.FromData<T>(data);
        }

    }

    [DataContract]
    public abstract class Response : Message
    {
        [DataMember]
        public bool status { get; set; }
        public Response(bool status)
        {
            this.status = status;
        }
        public new static T FromData<T>(string data)
        where T : Response
        {
            return Message.FromData<T>(data);
        }
    }

    [DataContract]
    public class ErrResponse : Response
    {
        [DataMember]
        public string message { get; set; }

        public ErrResponse(string message) : base(false)
        {
            this.message = message;
        }
    }

    [DataContract]
    public class ConvRequest : Request
    {
        [DataMember]
        public String converter;
        [DataMember]
        public String inputUnit;
        [DataMember]
        public String outputUnit;
        [DataMember]
        public String value;

        public ConvRequest(String converter, String inputUnit, String outputUnit, String value) : base(Method.Convert)
        {
            this.converter = converter;
            this.inputUnit = inputUnit;
            this.outputUnit = outputUnit;
            this.value = value;
        }
    }

    [DataContract]
    public class ConvResponse : Response
    {
        [DataMember]
        public string value;

        public ConvResponse(string value) : base(true)
        {
            this.value = value;
        }
    }

    [DataContract]
    public class ExchangeRateRequest : Request
    {
        [DataMember]
        public String currency;

        public ExchangeRateRequest(String currency) : base(Method.ExchangeRate)
        {
            this.currency = currency;
        }
    }

    [DataContract]
    public class ExchangeRateResponse : Response
    {
        [DataMember]
        public Dictionary<String, Double> rates;

        public ExchangeRateResponse(Dictionary<String, Double> rates) : base(true)
        {
            this.rates = rates;
        }
    }


    [DataContract]
    public class ConvListRequest : Request
    {
        public ConvListRequest() : base(Method.ConverterList)
        { }
    }

    [DataContract]
    public class ConvListResponse : Response
    {
        [DataMember]
        public Dictionary<string, List<Unit>> converters;

        public ConvListResponse(Dictionary<string, List<Unit>> converters) : base(true)
        {
            this.converters = converters;
        }
    }

}

