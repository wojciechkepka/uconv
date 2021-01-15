using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static UConv.Core.Units;

namespace UConv.Core
{
    [DataContract]
    public static class ExchangeRates
    {
        [DataMember]
        public static Dictionary<Unit, Dictionary<Unit, double>> Rates = new Dictionary<Unit, Dictionary<Unit, double>>
        {
            {
                Unit.RUB,
                new Dictionary<Unit, double>
                {
                    { Unit.PLN, 0.0 },
                    { Unit.EUR, 0.0 },
                    { Unit.USD, 0.0 },
                    { Unit.HUF, 0.0 },
                }
            },
            {
                Unit.PLN,
                new Dictionary<Unit, double>
                {
                    { Unit.RUB, 0.0 },
                    { Unit.EUR, 0.0 },
                    { Unit.USD, 0.0 },
                    { Unit.HUF, 0.0 },
                }
            },
            {
                Unit.EUR,
                new Dictionary<Unit, double>
                {
                    { Unit.RUB, 0.0 },
                    { Unit.PLN, 0.0 },
                    { Unit.USD, 0.0 },
                    { Unit.HUF, 0.0 },
                }
            },
            {
                Unit.USD,
                new Dictionary<Unit, double>
                {
                    { Unit.RUB, 0.0 },
                    { Unit.PLN, 0.0 },
                    { Unit.EUR, 0.0 },
                    { Unit.HUF, 0.0 },
                }
            },
            {
                Unit.HUF,
                new Dictionary<Unit, double>
                {
                    { Unit.RUB, 0.0 },
                    { Unit.PLN, 0.0 },
                    { Unit.EUR, 0.0 },
                    { Unit.USD, 0.0 },
                }
            }
        };

        public static void SetRandomRates()
        {
            Random rand = new Random();
            foreach(var unit in Rates)
            {
                foreach(var unit2 in unit.Value)
                {
                    Rates[unit.Key][unit2.Key] = rand.NextDouble() * (2 * rand.NextDouble());
                }
            }
        }
    }
    public class CurrencyConverter : IConverter<double, Unit>
    {
        public string Name => "Currency";

        public List<Unit> SupportedUnits => new()
        {
            Unit.RUB,
            Unit.PLN,
            Unit.EUR,
            Unit.USD,
            Unit.HUF,
        };

        public Tuple<double, Unit> Convert(double val, Unit inpUnit, Unit outUnit)
        {
            if (inpUnit == outUnit) return new Tuple<double, Unit>(val, outUnit);

            switch (inpUnit)
            {
                case Unit.RUB:
                case Unit.PLN:
                case Unit.EUR:
                case Unit.USD:
                case Unit.HUF:
                    switch (outUnit)
                    {
                        case Unit.RUB:
                        case Unit.PLN:
                        case Unit.EUR:
                        case Unit.USD:
                        case Unit.HUF:
                            if (inpUnit == outUnit) return new Tuple<double, Unit>(val, outUnit);
                            var rate = ExchangeRates.Rates[inpUnit][outUnit];
                            return new Tuple<double, Unit>(val * rate, outUnit);
                        default:
                            throw new IncompatibleConversionUnits(inpUnit, outUnit);
                    }
                default:
                    throw new UnsupportedUnit(inpUnit);
            }
        }
    }
}