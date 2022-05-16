using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocEditor.Model;

namespace DocEditor.ViewModel
{
    public class DocPaper : ViewModelBase
    {
        #region Private fields and Public properties
        /// <summary>
        /// The index number of the page
        /// </summary>
        private int _pageNumber;
        public int PageNumber { 
            get { return _pageNumber; }
            set
            {
                if (_pageNumber != value)
                {
                    _pageNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Page margin (same at the whole document)
        /// </summary>
        private double _rightMargin;
        public double RightMargin
        {
            get { return _rightMargin; }
            set
            {
                if (_rightMargin != value)
                {
                    _rightMargin = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _leftMargin;
        public double LeftMargin
        {
            get { return _leftMargin; }
            set
            {
                if (_leftMargin != value)
                {
                    _leftMargin = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _topMargin;
        public double TopMargin
        {
            get { return _topMargin; }
            set
            {
                if (_topMargin != value)
                {
                    _topMargin = value;
                    OnPropertyChanged();
                }
            }
        }
        private double _bottomMargin;
        public double BottomMargin
        {
            get { return _bottomMargin; }
            set
            {
                if (_bottomMargin != value)
                {
                    _bottomMargin = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The text formatting which is default at the beginning (color, size, font family, weight, style)
        /// </summary>
        private string _defaultColor;
        public string DefaultColor
        {
            get { return _defaultColor; }
            set
            {
                if (_defaultColor != value)
                {
                    _defaultColor = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _defaultFamily;
        public string DefaultFamily
        {
            get { return _defaultFamily; }
            set
            {
                if (_defaultFamily != value)
                {
                    _defaultFamily = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _defaultWeight;
        public string DefaultWeight
        {
            get { return _defaultWeight; }
            set
            {
                if (_defaultWeight != value)
                {
                    _defaultWeight = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _defaultStyle;
        public string DefaultStyle
        {
            get { return _defaultStyle; }
            set
            {
                if (_defaultStyle != value)
                {
                    _defaultStyle = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _defaultSize;
        public int DefaultSize
        {
            get { return _defaultSize; }
            set
            {
                if (_defaultSize != value)
                {
                    _defaultSize = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The current page for focus and caret position
        /// </summary>
        public bool isMainPage { get; set; }

        /// <summary>
        /// The size of the page (all pages have the same size)
        /// </summary>
        private int _sizeX;
        public int SizeX
        {
            get { return _sizeX; }
            set
            {
                if (_sizeX != value)
                {
                    _sizeX = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _sizeY;
        public int SizeY
        {
            get { return _sizeY; }
            set
            {
                if (_sizeY != value)
                {
                    _sizeY = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Selection highlight enable - always true
        /// </summary>
        private bool _selh = true;
        public bool SelectionHighLight { get { return _selh; } private set { } }

        //   <RichTextBox x:Name="DocPaper" FontFamily="{Binding FF}" FontSize="{Binding SIZE}" FontStyle="{Binding FS}" FontWeight="{Binding FW}" Padding="{Binding PageMargins}" HorizontalContentAlignment="Center"  Foreground="{Binding FC}" IsInactiveSelectionHighlightEnabled="True"  Background="#FFFFFF" BorderThickness="0" Margin="10 10 10 0" ScrollViewer.HorizontalScrollBarVisibility="Off" ScrollViewer.VerticalScrollBarVisibility="Auto">

        #endregion
    }
}
