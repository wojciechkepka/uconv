using System;
using System.Collections.Generic;
using static UConv.Core.Units;
using static UConv.Core.Formulas;

namespace UConv.Core
{
    public class CurrencyConverter : IConverter<double, Unit>
    {
        public string Name => "Currency";
        public List<Unit> SupportedUnits => new List<Unit>() {
            Unit.RUB,
            Unit.PLN,
            Unit.EUR,
            Unit.USD,
            Unit.HUF,
            Unit.CZK,
            Unit.JPY,
            Unit.GBP,
            Unit.BGN,
            Unit.ZAR,
        };
        public Tuple<double, Unit> Convert(double val, Unit inpUnit, Unit outUnit)
        {
            if (inpUnit == outUnit)
            {
                return new Tuple<double, Unit>(val, outUnit);
            }

            double outVal = 0;

            switch (inpUnit)
            {
                case Unit.RUB:
                case Unit.PLN:
                case Unit.EUR:
                case Unit.USD:
                case Unit.HUF:
                case Unit.CZK:
                case Unit.JPY:
                case Unit.GBP:
                case Unit.BGN:
                case Unit.ZAR:
                    switch (outUnit)
                    {
                        case Unit.RUB:
                        case Unit.PLN:
                        case Unit.EUR:
                        case Unit.USD:
                        case Unit.HUF:
                        case Unit.CZK:
                        case Unit.JPY:
                        case Unit.GBP:
                        case Unit.BGN:
                        case Unit.ZAR:
                            return new Tuple<double, Unit>(outVal, outUnit);
                        default:
                            throw new IncompatibleConversionUnits(inpUnit, outUnit);
                    }
                default:
                    throw new UnsupportedUnit(inpUnit);
            }
        }
    }
}
