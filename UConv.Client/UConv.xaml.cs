using System;
using System.Collections.Generic;
using System.Windows;
using UConv.Core;
using static UConv.Core.Units;

namespace UConv.Client
{
    public partial class MainWindow : Window
    {
        List<IConverter<double, Unit>> converters = new List<IConverter<double, Unit>>
            {
                new DistanceConverter(),
                new MassConverter(),
                new TemperatureConverter(),
                new SpeedConverter(),
            };
        TimeConverter tConv = new TimeConverter();
        bool timeConversion = false;
        double hours; double minutes;
        const int MaxRecordsPerPage = 20;
        List<string> converterNames = new List<string>() {
                "Distance",
                "Mass",
                "Temperature",
                "Speed",
                "Time",
        };
        public MainWindow()
        {
            InitializeComponent();
        }

    }
}
