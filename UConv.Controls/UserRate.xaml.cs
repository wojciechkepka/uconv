using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace UConv.Controls
{
    public partial class UserRate : UserControl
    {
        private readonly Tuple<byte, byte, byte, byte> Gold = new(0xFF, 0xFF, 0xD7, 0x00);
        public int userRating;

        public UserRate()
        {
            InitializeComponent();
            foreach (StarButton b in starGrid.Children) b.StarClick += StarButton_Click;
        }

        public event EventHandler<UserRatingEventArgs> UserRatingChanged;

        public void ResetColor()
        {
            foreach (StarButton b in starGrid.Children) b.DefaultFill();
        }

        public void SetColor(int n)
        {
            var i = 1;
            foreach (StarButton b in starGrid.Children)
            {
                if (i <= n) b.Fill = Gold;
                i++;
            }
        }

        private void StarButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ResetColor();
            var col = ((StarButton) sender).GetValue(Grid.ColumnProperty);
            var i = 0;
            SetColor((int) col + 1);
        }

        private void StarButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetColor();
            SetColor(userRating);
        }

        private void StarButton_Click(object sender, EventArgs e)
        {
            var col = ((StarButton) sender).GetValue(Grid.ColumnProperty);
            userRating = (int) col + 1;
            UserRatingChanged(sender, new UserRatingEventArgs {Rating = userRating});
        }

        public class UserRatingEventArgs : EventArgs
        {
            public int Rating { get; set; }
        }
    }
}