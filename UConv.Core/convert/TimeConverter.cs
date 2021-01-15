using System;
using System.Collections.Generic;
using static UConv.Core.Convert.Formulas;
using static UConv.Core.Units;

namespace UConv.Core.Convert
{
    public class TimeConverter : IConverter<string, TimeFormat>
    {
        public string Name => "Time";

        public List<TimeFormat> SupportedUnits => new()
        {
            TimeFormat.TwelveHour,
            TimeFormat.TwentyFourHour
        };

        public Tuple<string, TimeFormat> Convert(string val, TimeFormat inpFormat, TimeFormat outFormat)
        {
            if (inpFormat == TimeFormat.TwelveHour)
                return new Tuple<string, TimeFormat>(TwelveHToTwentyFour(val), TimeFormat.TwentyFourHour);
            return new Tuple<string, TimeFormat>(TwentyFourHToTwelveH(val), TimeFormat.TwelveHour);
        }
    }
}