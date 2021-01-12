﻿using System;
using System.Collections.Generic;
using static UConv.Core.Units;
using static UConv.Core.Formulas;

namespace UConv.Core
{
    public class TemperatureConverter : IConverter<double, Unit>
    {
        public string Name => "Temperature";
        public List<Unit> SupportedUnits => new List<Unit>() {
            Unit.Celsius,
            Unit.Fahrenheit,
            Unit.Kelvin,
        };
        public Tuple<double, Unit> Convert(double val, Unit inpUnit, Unit outUnit)
        {
            if (inpUnit == outUnit)
            {
                return new Tuple<double, Unit>(val, outUnit);
            }

            double outVal = 0;
            bool calculated = false;

            switch (inpUnit)
            {
                case Unit.Celsius:
                    if (outUnit == Unit.Fahrenheit)
                    {
                        outVal = CelsiusToFahrenheit(val);
                        calculated = true;
                    }
                    else if (outUnit == Unit.Kelvin)
                    {
                        outVal = CelsiusToKelvin(val);
                        calculated = true;
                    }
                    break;
                case Unit.Fahrenheit:
                    if (outUnit == Unit.Celsius)
                    {
                        outVal = FahrenheitToCelsius(val);
                        calculated = true;
                    }
                    else if (outUnit == Unit.Kelvin)
                    {
                        outVal = FahrenheitToKelvin(val);
                        calculated = true;
                    }
                    break;
                case Unit.Kelvin:
                    if (outUnit == Unit.Celsius)
                    {
                        outVal = KelvinToCelsius(val);
                        calculated = true;
                    }
                    else if (outUnit == Unit.Fahrenheit)
                    {
                        outVal = KelvinToFahrenheit(val);
                        calculated = true;
                    }
                    break;
                default:
                throw new UnsupportedUnit(inpUnit);
            }

            if (calculated)
            {
                return new Tuple<double, Unit>(outVal, outUnit);
            }
            else
            {
                throw new IncompatibleConversionUnits(inpUnit, outUnit);
            }
        }
    }
}
