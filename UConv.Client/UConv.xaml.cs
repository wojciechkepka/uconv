﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UConv.Controls;
using UConv.Core;
using static UConv.Core.Units;

namespace UConv.Client
{
    public partial class MainWindow : Window
    {
        private readonly ConvClient client;

        private Dictionary<string, List<Unit>> converters;

        public MainWindow()
        {
            InitializeComponent();
            client = new ConvClient(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), 7001);
            getConverters();
            getLastRating();
            convComboBox.ItemsSource = converters.Keys;
            userRateControl.UserRatingChanged += userRatingChangedHandler;
        }

        private void getLastRating()
        {
            var t = new Thread(() =>
            {
                var resp = client.LastRatingRequest(Dns.GetHostName());
                if (typeof(ErrResponse) == resp.GetType())
                {
                    Dispatcher.Invoke(() => { setError(((ErrResponse) resp).message); });
                }
                else
                {
                    var rateResp = (LastRatingResponse) resp;
                    Dispatcher.Invoke(() =>
                    {
                        userRateControl.userRating = rateResp.rating;
                        userRateControl.ResetColor();
                        userRateControl.SetColor(userRateControl.userRating);
                    });
                }
            });
            t.Start();
        }

        private void getConverters()
        {
            var resp = client.ConverterListRequest();
            if (typeof(ErrResponse) == resp.GetType())
                setError(((ErrResponse) resp).message);
            else
                converters = ((ConvListResponse) resp).converters;
        }

        private void setMessage(string message)
        {
            outputTextBlock.Foreground = Brushes.Black;
            outputTextBlock.Text = message;
        }

        private void setError(string message)
        {
            setMessage(message);
            outputTextBlock.Foreground = Brushes.Red;
        }

        private void convComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var conv = convComboBox.SelectedItem.ToString();
            inpUnitComboBox.ItemsSource = converters.GetValueOrDefault(conv);
            outUnitComboBox.ItemsSource = converters.GetValueOrDefault(conv);
        }

        private void userRatingChangedHandler(object sender, UserRate.UserRatingEventArgs args)
        {
            try
            {
                var resp = client.RateMeRequest(Dns.GetHostName(), args.Rating);
                if (typeof(ErrResponse) == resp.GetType())
                    setError(((ErrResponse) resp).message);
                else
                    userRateControl.SetColor(args.Rating);
            }
            catch (Exception ex)
            {
                setError(ex.Message);
            }
        }

        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            if (convComboBox.SelectedIndex < 0)
            {
                setError("Converter not selected");
                return;
            }

            if (inpUnitComboBox.SelectedIndex < 0)
            {
                setError("Input unit not selected");
                return;
            }

            if (outUnitComboBox.SelectedIndex < 0)
            {
                setError("Output unit not selected");
                return;
            }

            if (userInputBox.Text == "")
            {
                setError("Input box is empty");
                return;
            }

            try
            {
                var resp = client.ConvertRequest(convComboBox.SelectedItem.ToString(),
                    inpUnitComboBox.SelectedItem.ToString(), outUnitComboBox.SelectedItem.ToString(),
                    userInputBox.Text);
                if (typeof(ErrResponse) == resp.GetType())
                {
                    setError(((ErrResponse) resp).message);
                }
                else
                {
                    var r = (ConvResponse) resp;
                    setMessage($"{r.value} {UnitSymbol((Unit) outUnitComboBox.SelectedItem)}");
                }
            }
            catch (Exception ex)
            {
                setError(ex.Message);
            }
        }
    }
}