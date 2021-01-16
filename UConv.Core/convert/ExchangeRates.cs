using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static UConv.Core.Units;

namespace UConv.Core.Convert
{
    [DataContract]
    public static class ExchangeRates
    {
        [DataMember] public static Dictionary<Unit, Dictionary<Unit, double>> Rates = new()
        {
            {
                Unit.RUB,
                new Dictionary<Unit, double>
                {
                    {Unit.PLN, 0.0},
                    {Unit.EUR, 0.0},
                    {Unit.USD, 0.0},
                    {Unit.HUF, 0.0}
                }
            },
            {
                Unit.PLN,
                new Dictionary<Unit, double>
                {
                    {Unit.RUB, 0.0},
                    {Unit.EUR, 0.0},
                    {Unit.USD, 0.0},
                    {Unit.HUF, 0.0}
                }
            },
            {
                Unit.EUR,
                new Dictionary<Unit, double>
                {
                    {Unit.RUB, 0.0},
                    {Unit.PLN, 0.0},
                    {Unit.USD, 0.0},
                    {Unit.HUF, 0.0}
                }
            },
            {
                Unit.USD,
                new Dictionary<Unit, double>
                {
                    {Unit.RUB, 0.0},
                    {Unit.PLN, 0.0},
                    {Unit.EUR, 0.0},
                    {Unit.HUF, 0.0}
                }
            },
            {
                Unit.HUF,
                new Dictionary<Unit, double>
                {
                    {Unit.RUB, 0.0},
                    {Unit.PLN, 0.0},
                    {Unit.EUR, 0.0},
                    {Unit.USD, 0.0}
                }
            }
        };

        public static void SetRandomRates()
        {
            var rand = new Random();
            foreach (var unit in Rates)
            foreach (var unit2 in unit.Value)
                Rates[unit.Key][unit2.Key] = rand.NextDouble() * (10 * rand.NextDouble()) / (10 * rand.NextDouble());
        }
    }
}
