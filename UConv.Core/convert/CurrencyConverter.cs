using System;
using System.Collections.Generic;
using static UConv.Core.Units;

namespace UConv.Core.Convert
{
    public class CurrencyConverter : IConverter<double, Unit>
    {
        public string Name => "Currency";

        public List<Unit> SupportedUnits => new()
        {
            Unit.RUB,
            Unit.PLN,
            Unit.EUR,
            Unit.USD,
            Unit.HUF
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