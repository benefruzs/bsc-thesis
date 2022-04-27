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
    /// Interaction logic for Spacing.xaml
    /// </summary>
    public partial class Spacing : Window
    {
        public Spacing()
        {
            InitializeComponent();
            left.Text = _numValueLeft.ToString();
            right.Text = _numValueRight.ToString();
            lineSpace.Text = _numValueLine.ToString();
        }

        private double _numValueLeft = 0;
        private double _numValueRight = 0;
        private double _numValueLine = 0;

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

        public double NumValueLine
        {
            get { return _numValueLine; }
            set
            {
                _numValueLine = value;
                lineSpace.Text = value.ToString();
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

        private void lineSpace_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lineSpace == null)
            {
                return;
            }

            if (!double.TryParse(lineSpace.Text, out _numValueLine))
                lineSpace.Text = _numValueLine.ToString();
        }

        private void cmdUpLine_Click(object sender, RoutedEventArgs e)
        {
            NumValueLine += 0.1;
            NumValueLine = (double)Math.Round(NumValueLine, 1, MidpointRounding.AwayFromZero);
        }

        private void cmdDownLine_Click(object sender, RoutedEventArgs e)
        {
            NumValueLine -= 0.1;
            NumValueLine = (double)Math.Round(NumValueLine, 1, MidpointRounding.AwayFromZero);
        }
    }
}
