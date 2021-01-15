﻿using System;
using System.Collections.Generic;

namespace UConv.Core.Convert
{
    public interface IConverter<T, U>
    {
        string Name { get; }
        List<U> SupportedUnits { get; }
        Tuple<T, U> Convert(T val, U inpUnit, U outUnit);
    }
}