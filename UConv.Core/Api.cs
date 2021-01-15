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
        ExchangeRate,
        SaveRating,
        LastRating,
    }

    [DataContract]
    public abstract class Message
    {

        public static T FromData<T>(String data)
        where T : Message
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(T), typeof(T).Name.ToString(), "");
            var sr = new StringReader(data);
            var xr = new XmlTextReader(sr);
            return (T)ser.ReadObject(xr);
        }

        public String ToXmlString<T>()
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

        public new static T FromData<T>(String data)
        where T : Request
        {
            return Message.FromData<T>(data);
        }

    }

    [DataContract]
    public abstract class Response : Message
    {
        public bool status { get; set; }
        public Response(bool status)
        {
            this.status = status;
        }
        public new static T FromData<T>(String data)
        where T : Response
        {
            return Message.FromData<T>(data);
        }
    }

    [DataContract]
    public class ErrResponse : Response
    {
        public String message { get; set; }

        public ErrResponse(String message) : base(false)
        {
            this.message = message;
        }
    }

    [DataContract]
    public class ConvRequest : Request
    {
        public String converter;
        public String inputUnit;
        public String outputUnit;
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
        public String value;

        public ConvResponse(String value) : base(true)
        {
            this.value = value;
        }
    }

    [DataContract]
    public class ExchangeRateRequest : Request
    {
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

    [DataContract]
    public class RateMeRequest : Request
    {
        public string hostname;
        public int rating;

        public RateMeRequest(string hostname, int rating) : base(Method.SaveRating)
        {
            this.hostname = hostname;
            this.rating = rating;
        }
    }


    [DataContract]
    public class RateMeResponse : Response
    {
        public RateMeResponse() : base(true)
        {
        }
    }

    [DataContract]
    public class LastRatingRequest : Request
    {
        public string hostname;

        public LastRatingRequest(string hostname) : base(Method.LastRating)
        {
            this.hostname = hostname;
        }
    }

    [DataContract]
    public class LastRatingResponse : Response
    {
        public DateTime date;
        public string hostname;
        public int rating;


        public LastRatingResponse(DateTime date, String hostname, int rating) : base(true)
        {
            this.date = date;
            this.hostname = hostname;
            this.rating = rating;
        }
    }
}

