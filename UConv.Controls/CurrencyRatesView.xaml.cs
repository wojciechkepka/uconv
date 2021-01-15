using System.Collections.Generic;
using System.Windows.Controls;

namespace UConv.Controls
{
    public partial class CurrencyRatesView : UserControl
    {
        protected readonly Dictionary<string, double> Rates;

        public CurrencyRatesView(Dictionary<string, double> rates)
        {
            InitializeComponent();

            Rates = rates;
            foreach (var item in Rates)
            {
                var l = new Label {Content = $"{item.Key} = {item.Value}"};
                gridStack.Children.Add(l);
            }
        }
    }
}