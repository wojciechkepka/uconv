using System;
using System.Text;
using System.Collections.Generic;
using static UConv.Core.Units;
using static UConv.Core.Formulas;

namespace UConv.Core
{
    public class TimeConverter : IConverter<string, TimeFormat>
    {
        public string Name => "Distance Converter";
        public List<TimeFormat> SupportedUnits => new List<TimeFormat>() {
            TimeFormat.TwelveHour,
            TimeFormat.TwentyFourHour,
        };
        public Tuple<string, TimeFormat> Convert(string val, TimeFormat inpFormat, TimeFormat outFormat)
        {
            if (inpFormat == TimeFormat.TwelveHour)
            {
                return new Tuple<string, TimeFormat>(TwelveHToTwentyFour(val), TimeFormat.TwentyFourHour);
            }
            else
            {
                return new Tuple<string, TimeFormat>(TwentyFourHToTwelveH(val), TimeFormat.TwelveHour);
            }
        }
    }
}
