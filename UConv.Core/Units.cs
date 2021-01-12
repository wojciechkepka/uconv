using System;
using System.Collections.Generic;
using System.Text;

namespace UConv.Core
{
    public class Units
    {
        public enum TimeFormat
        {
            TwentyFourHour,
            TwelveHour,
        }
        public static TimeFormat TimeFormatFromString(string format)
        {
            switch (format)
            {
                case "h":
                    return TimeFormat.TwentyFourHour;
                case "am":
                case "pm":
                    return TimeFormat.TwelveHour;
                default:
                    throw new InvalidTimeFormat(format);
            }
        }
        public static TimeFormat OppositeFormat(TimeFormat format)
        {
            if (format == TimeFormat.TwelveHour)
            {
                return TimeFormat.TwentyFourHour;
            } else
            {
                return TimeFormat.TwelveHour;
            }
        }
        public enum Unit
        {
            Celsius,
            Fahrenheit,
            Kelvin,
            Kilometers,
            Miles,
            Kilograms,
            Pounds,
            Ounces,
            KilometersPerHour, // Kph
            MilesPerHour, // Mph
            MetersPerSecond, // Mps
            Knots,

            //

            RUB,
            PLN,
            EUR,
            USD,
            HUF,
            CZK,
            JPY,
            GBP,
            BGN,
            ZAR,
        }
        public static Unit UnitFromString(String unit)
        {
            switch (unit)
            {
                // Temperatures
                case "c":
                    return Unit.Celsius;
                case "f":
                    return Unit.Fahrenheit;
                case "k":
                    return Unit.Kelvin;
                // Mass
                case "kg":
                    return Unit.Kilograms;
                case "lb":
                    return Unit.Pounds;
                case "oz":
                    return Unit.Ounces;
                // Distance
                case "km":
                    return Unit.Kilometers;
                case "mi":
                    return Unit.Miles;
                // Speed
                case "km/h":
                    return Unit.KilometersPerHour;
                case "mi/h":
                    return Unit.MilesPerHour;
                case "m/s":
                    return Unit.MetersPerSecond;
                case "knots":
                    return Unit.Knots;
                // Currency
                case "₽":
                    return Unit.RUB;
                case "zł":
                case "zl":
                    return Unit.PLN;
                case "€":
                    return Unit.EUR;
                case "$":
                    return Unit.USD;
                case "Ft":
                    return Unit.HUF;
                case "Kč":
                    return Unit.CZK;
                case "¥":
                    return Unit.JPY;
                case "£":
                    return Unit.GBP;
                case "лв":
                    return Unit.BGN;
                case "R":
                    return Unit.ZAR;

                default:
                    throw new UnexpectedEnumValueException<Unit>(unit);
            }
        }
        public static string UnitSymbol(Unit unit)
        {
            switch (unit)
            {
                // Temperatures
                case Unit.Celsius:
                    return "C";
                case Unit.Fahrenheit:
                    return "F";
                case Unit.Kelvin:
                    return "K";
                // Mass
                case Unit.Kilograms:
                    return "kg";
                case Unit.Pounds:
                    return "lb";
                case Unit.Ounces:
                    return "oz";
                // Distance
                case Unit.Kilometers:
                    return "km";
                case Unit.Miles:
                    return "mi";
                // Speed
                case Unit.KilometersPerHour:
                    return "km/h";
                case Unit.MilesPerHour:
                    return "mi/h";
                case Unit.MetersPerSecond:
                    return "m/s";
                case Unit.Knots:
                    return "knots";
                // Currency
                case Unit.RUB:
                    return "₽";
                case Unit.PLN:
                    return "zł";
                case Unit.EUR:
                    return "€";
                case Unit.USD:
                    return "$";
                case Unit.HUF:
                    return "Ft";
                case Unit.CZK:
                    return "Kč";
                case Unit.JPY:
                    return "¥";
                case Unit.GBP:
                    return "£";
                case Unit.BGN:
                    return "лв";
                case Unit.ZAR:
                    return "R";

                default:
                    return "";
            }
        }
    }
}
