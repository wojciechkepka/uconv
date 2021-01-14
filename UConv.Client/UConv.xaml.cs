using System;
using System.Collections.Generic;
using System.Windows;
using UConv.Core;
using static UConv.Core.Units;
using UConv.Controls;
using System.Windows.Media;
using System.Net;

namespace UConv.Client
{
    public partial class MainWindow : Window
    {
        ConvClient client;

        Dictionary<string, List<Unit>> converters;

        public MainWindow()
        {
            InitializeComponent();
            client = new ConvClient(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), 7001);
            getConverters();
            convComboBox.ItemsSource = converters.Keys;
            userRateControl.UserRatingChanged += userRatingChangedHandler;
        }

        private void getConverters()
        {
            var resp = client.ConverterListRequest();
            if (typeof(ErrResponse) == resp.GetType())
            {
                setError(((ErrResponse)resp).message);
            }
            else
            {
                converters = ((ConvListResponse)resp).converters;
            }
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

        private void convComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var conv = convComboBox.SelectedItem.ToString();
            inpUnitComboBox.ItemsSource = converters.GetValueOrDefault(conv);
            outUnitComboBox.ItemsSource = converters.GetValueOrDefault(conv);
        }

        private void userRatingChangedHandler(object sender, UserRate.UserRatingEventArgs args)
        {

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

        }
    }
}
