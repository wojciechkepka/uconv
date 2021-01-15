using System;
using System.Collections.Generic;
using static UConv.Core.Formulas;
using static UConv.Core.Units;

namespace UConv.Core
{
    public class DistanceConverter : IConverter<double, Unit>
    {
        public string Name => "Distance";

        public List<Unit> SupportedUnits => new()
        {
            Unit.Kilometers,
            Unit.Miles
        };

        public Tuple<double, Unit> Convert(double val, Unit inpUnit, Unit outUnit)
        {
            if (inpUnit == outUnit) return new Tuple<double, Unit>(val, outUnit);

            double outVal = 0;
            var calculated = false;

            switch (inpUnit)
            {
                case Unit.Kilometers:
                    if (outUnit == Unit.Miles)
                    {
                        outVal = KilometersToMiles(val);
                        calculated = true;
                    }

                    break;
                case Unit.Miles:
                    if (outUnit == Unit.Kilometers)
                    {
                        outVal = MilesToKilometers(val);
                        calculated = true;
                    }

                    break;
                default:
                    throw new UnsupportedUnit(inpUnit);
            }

            if (calculated)
                return new Tuple<double, Unit>(outVal, outUnit);
            throw new IncompatibleConversionUnits(inpUnit, outUnit);
        }
    }
}