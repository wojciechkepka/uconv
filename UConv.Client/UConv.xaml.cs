using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using UConv.Controls;
using UConv.Core;
using UConv.Core.Net;
using UConv.Core.Db;
using static UConv.Core.Units;

namespace UConv.Client
{
    public partial class MainWindow : Window
    {
        private readonly UConvClient client;

        private Dictionary<string, List<Unit>> converters;
        private Thread clientThread;

        public MainWindow()
        {
            InitializeComponent();
            client = new UConvClient(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), 7001);
            getData();
            userRateControl.UserRatingChanged += userRatingChangedHandler;
        }

        private void getData()
        {
            if (clientThread != null && clientThread.IsAlive) return;
            clientThread = new Thread(async () =>
            {
                await getConverters();
                Dispatcher.Invoke(() =>
                {
                    convComboBox.ItemsSource = converters.Keys;
                });
                await getCurrencies();
                await getStats();
                await getLastRating();
            });
            clientThread.Start();

        }

        private async Task getCurrencies()
        {
            await Task.Run(() =>
            {
                var resp = client.CurrenciesListRequest();
                if (typeof(ErrResponse) == resp.GetType())
                {
                    Dispatcher.Invoke(() => { setError(((ErrResponse) resp).message); });
                }
                else
                {
                    var rateResp = (CurrencyListResponse) resp;
                    Dispatcher.Invoke(() =>
                    {
                        currencyComboBox.ItemsSource = rateResp.currencies;
                    });
                }
            });
        }

        private async Task getLastRating()
        {
            await Task.Run(() =>
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
        }

        private async Task getConverters()
        {
            await Task.Run(() =>
            {
                var resp = client.ConverterListRequest();
                if (typeof(ErrResponse) == resp.GetType())
                {
                    Dispatcher.Invoke(() =>
                    {
                        setError(((ErrResponse)resp).message);
                    });
                }
                else
                {
                    converters = ((ConvListResponse) resp).converters;
                }
            });
        }

        private async Task getStats()
        {
            await Task.Run(() =>
            {
                var resp = client.StatisticsRequest();
                if (typeof(ErrResponse) == resp.GetType())
                {
                    Dispatcher.Invoke(() =>
                    {
                        setError(((ErrResponse)resp).message);
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        statsDataGrid.ItemsSource = ((StatisticsResponse)resp).stats;
                    });
                }
            });
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

        private async void clearDataButton_Click(object sender, RoutedEventArgs e)
        {

            await Task.Run(() =>
            {
                var resp = client.ClearDataRequest();
                if (typeof(ErrResponse) == resp.GetType())
                {
                    Dispatcher.Invoke(() =>
                    {
                        setError(((ErrResponse)resp).message);
                    });
                }

            });
            getData();
        }

        private void currencyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currencyRateStackPanel.Children.Clear();
            try
            {
                var resp = client.ExchangeRatesRequest(currencyComboBox.SelectedItem.ToString());
                if (typeof(ErrResponse) == resp.GetType())
                {
                    setError(((ErrResponse)resp).message);
                    return;
                }

                var ratesResp = (ExchangeRateResponse)resp;
                foreach (var currency in ratesResp.rates)
                {
                    Grid g = new Grid { };
                    var nameCol = new ColumnDefinition();
                    nameCol.Width = new GridLength(30, GridUnitType.Star);
                    var valCol = new ColumnDefinition();
                    valCol.Width = new GridLength(70, GridUnitType.Star);
                    g.ColumnDefinitions.Add(nameCol);
                    g.ColumnDefinitions.Add(valCol);
                    Label name = new Label { Content = $"{currency.Key}    ", FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Left};
                    Label value = new Label { Content = currency.Value, FontStyle = FontStyles.Italic, HorizontalAlignment = HorizontalAlignment.Right };
                    name.SetValue(Grid.ColumnProperty, 0);
                    value.SetValue(Grid.ColumnProperty, 1);
                    g.Children.Add(name);
                    g.Children.Add(value);
                    currencyRateStackPanel.Children.Add(g);
                }

            }
            catch (Exception ex)
            {
                setError(ex.Message);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clientThread.IsAlive) return;

            var tab = ((TabItem)mainTabControl.SelectedItem).Header;
            clientThread = new Thread(async () =>
            {
                switch (tab)
                {
                    case "Converter": await getConverters(); break;
                    case "Statistics": await getStats(); break;
                    case "Settings": await getLastRating(); break;
                    case "Exchange rates": await getCurrencies(); break;
                }
            });
            clientThread.Start();
        }
    }
}