using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace UConv.Controls
{
    /// <summary>
    ///     Interaction logic for StarButton.xaml
    /// </summary>
    public partial class StarButton : UserControl
    {
        private Tuple<byte, byte, byte, byte> fill;

        public StarButton()
        {
            InitializeComponent();
            DefaultFill();
        }

        public Tuple<byte, byte, byte, byte> Fill
        {
            get => fill;
            set
            {
                fill = value;
                var brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(fill.Item1, fill.Item2, fill.Item3, fill.Item4);
                starPath.Fill = brush;
            }
        }

        public event EventHandler StarClick;

        public void DefaultFill()
        {
            var brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
            starPath.Fill = brush;
        }

        public void Button_Click(object sender, EventArgs e)
        {
            StarClick(this, e);
        }
    }
}