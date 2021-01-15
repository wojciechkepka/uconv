using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UConv.Controls
{
    public partial class CurrencyRatesView : UserControl
    {
        protected readonly Dictionary<string, double> Rates;

        public CurrencyRatesView(Dictionary<string, double> rates)
        {
            InitializeComponent();

            Rates = rates;
            foreach(var item in Rates)
            {
                Label l = new Label { Content = $"{item.Key} = {item.Value}" };
                gridStack.Children.Add(l);
            }
        }
    }
}
