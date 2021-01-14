﻿using System;

namespace UConv.Core
{
    public class UnexpectedEnumValueException<T> : Exception
    {
        public UnexpectedEnumValueException(string value)
            : base("Value" + value + " is not supported for " + typeof(T).Name)
        { }
    }
    public class IncompatibleConversionUnits : Exception
    {
        public IncompatibleConversionUnits(Units.Unit inUnit, Units.Unit outUnit)
            : base($"Incompatible units. Can't convert from {inUnit.ToString()} to {inUnit.ToString()}")
        { }
    }
    public class UnsupportedUnit : Exception
    {
        public UnsupportedUnit(Units.Unit unit)
            : base($"Unsupported unit {unit.ToString()}")
        { }
    }
    public class InvalidTimeFormat : Exception
    {
        public InvalidTimeFormat(string time)
            : base($"Invalid time format '{time}'")
        { }
    }

    public class DbError : Exception
    {
        public DbError(string message)
            : base($"Error with database: {message}")
        { }
    }
}
