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
using System.Windows.Shapes;

namespace DocEditor
{
    /// <summary>
    /// Interaction logic for PageSettingsWindow.xaml
    /// </summary>
    public partial class PageSettingsWindow : Window
    {
        public PageSettingsWindow()
        {
            InitializeComponent();
            left.Text = _numValueLeft.ToString();
            right.Text = _numValueRight.ToString();
            top.Text = _numValueTop.ToString();
            bottom.Text = _numValueBottom.ToString();
        }

        private double _numValueLeft = 0;
        private double _numValueRight = 0;
        private double _numValueTop = 0;
        private double _numValueBottom = 0;

        public double NumValueLeft
        {
            get { return _numValueLeft; }
            set
            {
                _numValueLeft = value;
                left.Text = value.ToString();
            }
        }

        public double NumValueRight
        {
            get { return _numValueRight; }
            set
            {
                _numValueRight = value;
                right.Text = value.ToString();
            }
        }

        public double NumValueTop
        {
            get { return _numValueTop; }
            set
            {
                _numValueTop = value;
                top.Text = value.ToString();
            }
        }

        public double NumValueBottom
        {
            get { return _numValueBottom; }
            set
            {
                _numValueBottom = value;
                bottom.Text = value.ToString();
            }
        }

        private void cmdUpleft_Click(object sender, RoutedEventArgs e)
        {
            NumValueLeft += 0.1;
            NumValueLeft = (double)Math.Round(NumValueLeft, 1, MidpointRounding.AwayFromZero);
        }

        private void cmdDownleft_Click(object sender, RoutedEventArgs e)
        {
            NumValueLeft -= 0.1;
            NumValueLeft = (double)Math.Round(NumValueLeft, 1, MidpointRounding.AwayFromZero);
        }

        private void left_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (left == null)
            {
                return;
            }

            if (!double.TryParse(left.Text, out _numValueLeft))
                left.Text = _numValueLeft.ToString();
        }

        private void cmdUpright_Click(object sender, RoutedEventArgs e)
        {
            NumValueRight += 0.1;
            NumValueRight = (double)Math.Round(NumValueRight, 1, MidpointRounding.AwayFromZero);
        }

        private void cmdDownright_Click(object sender, RoutedEventArgs e)
        {
            NumValueRight -= 0.1;
            NumValueRight = (double)Math.Round(NumValueRight, 1, MidpointRounding.AwayFromZero);
        }

        private void right_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (right == null)
            {
                return;
            }

            if (!double.TryParse(right.Text, out _numValueRight))
                right.Text = _numValueRight.ToString();
        }

        private void top_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (top == null)
            {
                return;
            }

            if (!double.TryParse(top.Text, out _numValueTop))
                top.Text = _numValueTop.ToString();
        }

        private void cmdUptop_Click(object sender, RoutedEventArgs e)
        {
            NumValueTop += 0.1;
            NumValueTop = (double)Math.Round(NumValueTop, 1, MidpointRounding.AwayFromZero);
        }

        private void cmdDowntop_Click(object sender, RoutedEventArgs e)
        {
            NumValueTop -= 0.1;
            NumValueTop = (double)Math.Round(NumValueTop, 1, MidpointRounding.AwayFromZero);
        }

        private void bottom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (top == null)
            {
                return;
            }

            if (!double.TryParse(bottom.Text, out _numValueBottom))
                bottom.Text = _numValueBottom.ToString();
        }

        private void cmdUpbottom_Click(object sender, RoutedEventArgs e)
        {
            NumValueBottom += 0.1;
            NumValueBottom = (double)Math.Round(NumValueBottom, 1, MidpointRounding.AwayFromZero);
        }

        private void cmdDownbottom_Click(object sender, RoutedEventArgs e)
        {
            NumValueBottom -= 0.1;
            NumValueBottom = (double)Math.Round(NumValueTop, 1, MidpointRounding.AwayFromZero);
        }
    }
}
