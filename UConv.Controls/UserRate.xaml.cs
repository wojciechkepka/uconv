using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace UConv.Controls
{
    public partial class UserRate : UserControl
    {
        private readonly Tuple<byte, byte, byte, byte> Gold = new Tuple<byte, byte, byte, byte>(0xFF, 0xFF, 0xD7, 0x00);

        public event EventHandler<UserRatingEventArgs> UserRatingChanged;
        public int userRating = 0;
        
        public UserRate()
        {
            InitializeComponent();
            foreach (StarButton b in starGrid.Children)
            {
                b.StarClick += new EventHandler(StarButton_Click);
            }
        }

        public class UserRatingEventArgs : EventArgs
        {
            public int Rating
            {
                get;
                set;
            }
        }

        public void ResetColor()
        {
            foreach (StarButton b in starGrid.Children)
            {
                b.DefaultFill();
            }
        }

        public void SetColor(int n)
        {
            int i = 1;
            foreach (StarButton b in starGrid.Children)
            {
                if (i <= n)
                {
                    b.Fill = Gold;
                }
                i++;
            }
        }

        private void StarButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ResetColor();
            var col = ((StarButton)sender).GetValue(Grid.ColumnProperty);
            int i = 0;
            SetColor((int)col + 1);
        }

        private void StarButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetColor();
            SetColor(userRating);
        }

        private void StarButton_Click(object sender, EventArgs e)
        {
            var col = ((StarButton)sender).GetValue(Grid.ColumnProperty);
            this.userRating = (int)col + 1;
            UserRatingChanged(sender, new UserRatingEventArgs() { Rating = this.userRating });
        }

    }
}
