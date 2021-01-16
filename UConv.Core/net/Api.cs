using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UConv.Core.Db;
using static UConv.Core.Units;

namespace UConv.Core.Net
{
    public enum Method
    {
        ConverterList,
        Convert,
        ExchangeRate,
        SaveRating,
        LastRating,
        ClearData,
        CurrencyList,
        Statistics
    }

    [DataContract]
    [KnownType(typeof(ErrResponse))]
    public class ErrResponse : Response
    {
        public ErrResponse(string message) : base(false)
        {
            this.message = message;
        }

        [DataMember] public string message { get; set; }
    }

    [DataContract]
    public class ConvRequest : Request
    {
        [DataMember] public string converter;

        [DataMember] public string inputUnit;

        [DataMember] public string outputUnit;

        [DataMember] public string value;

        public ConvRequest(string converter, string inputUnit, string outputUnit, string value) : base(Method.Convert)
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
        [DataMember] public string value;

        public ConvResponse(string value) : base(true)
        {
            this.value = value;
        }
    }

    [DataContract]
    public class ExchangeRateRequest : Request
    {
        [DataMember] public string currency;

        public ExchangeRateRequest(string currency) : base(Method.ExchangeRate)
        {
            this.currency = currency;
        }
    }

    [DataContract]
    public class ExchangeRateResponse : Response
    {
        [DataMember] public Dictionary<Unit, double> rates;

        public ExchangeRateResponse(Dictionary<Unit, double> rates) : base(true)
        {
            this.rates = rates;
        }
    }


    [DataContract]
    public class ConvListRequest : Request
    {
        public ConvListRequest() : base(Method.ConverterList)
        {
        }
    }

    [DataContract]
    public class ConvListResponse : Response
    {
        [DataMember] public Dictionary<string, List<Unit>> converters;

        public ConvListResponse(Dictionary<string, List<Unit>> converters) : base(true)
        {
            this.converters = converters;
        }
    }

    [DataContract]
    public class RateMeRequest : Request
    {
        [DataMember] public string hostname;

        [DataMember] public int rating;

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
        [DataMember] public string hostname;

        public LastRatingRequest(string hostname) : base(Method.LastRating)
        {
            this.hostname = hostname;
        }
    }

    [DataContract]
    public class LastRatingResponse : Response
    {
        [DataMember] public DateTime date;

        [DataMember] public string hostname;

        [DataMember] public int rating;


        public LastRatingResponse(DateTime date, string hostname, int rating) : base(true)
        {
            this.date = date;
            this.hostname = hostname;
            this.rating = rating;
        }
    }

    [DataContract]
    public class ClearDataRequest : Request
    {
        [DataMember] public string hostname;

        public ClearDataRequest(string hostname) : base(Method.ClearData)
        {
            this.hostname = hostname;
        }
    }

    [DataContract]
    public class ClearDataResponse : Response
    {
        public ClearDataResponse() : base(true)
        {
        }
    }

    [DataContract]
    public class CurrencyListRequest : Request
    {
        public CurrencyListRequest() : base(Method.CurrencyList)
        {
        }
    }

    [DataContract]
    public class CurrencyListResponse : Response
    {
        [DataMember] public List<string> currencies;

        public CurrencyListResponse(List<string> currencies) : base(true)
        {
            this.currencies = currencies;
        }
    }

    [DataContract]
    public class StatisticsRequest : Request
    {
        public StatisticsRequest() : base(Method.Statistics)
        {
        }
    }

    [DataContract]
    public class StatisticsResponse : Response
    {
        [DataMember] public List<Record> stats;

        public StatisticsResponse(List<Record> stats) : base(true)
        {
            this.stats = stats;
        }
    }
}