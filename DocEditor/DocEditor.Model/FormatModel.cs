using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DocEditor.Model
{
    public class FormatModel : IFormatModel
    {
        #region Private fields and Public properties
        /// <summary>
        /// Font style: Italic, Normal, Oblique
        /// </summary>
        private string _fontStyle;
        public string Style
        {
            get { return _fontStyle; }
            //set { OnPropertyChanged(ref _fontStyle, value); }
            set
            {
                if (_fontStyle != value) { _fontStyle = value; OnFormatChanged(); }
            }
        }

        /// <summary>
        /// Font weight: black, bold, demiBold, extraBlack, extraLight, 
        /// heavy, light, medium, normal, regular, semiBold, 
        /// thin, ultraBlack, ultraBold, ultraLight 
        /// </summary>
        private string _fontWeight;
        public string Weight
        {
            get { return _fontWeight; }
            set {
                if (_fontWeight != value) { _fontWeight = value; OnFormatChanged(); }
            }
        }

        /// <summary>
        /// Font family
        /// </summary>
        private string _fontFamily;
        public string Family
        {
            get { return _fontFamily; }
            set {
                if (_fontFamily != value) { _fontFamily = value; OnFormatChanged(); }
            }
        }

        /// <summary>
        /// Character offset: -2 subscript, 2 superscript, 1 normal
        /// </summary>
        private int _charOffset;
        public int CharOffset
        {
            get { return _charOffset; }
            set { 
                if (_charOffset != value) { _charOffset = value; OnFormatChanged(); } 
            }
        }

        /// <summary>
        /// Font size
        /// </summary>
        private int _size;
        public int Size
        {
            get { return _size; }
            set {
                if (_size != value) { _size = value; OnFormatChanged(); }
            }
        }

        /// <summary>
        /// Font color
        /// </summary>
        private string _color;
        public string Color
        {
            get { return _color; }
            set
            {
                if(_color != value) { _color = value; OnFormatChanged(); }
            }
        }

        private string _textDecoration;
        public string TextDecoration
        {
            get { return _textDecoration; }
            set
            {
                if (_textDecoration != value) { _textDecoration = value; OnFormatChanged(); }
            }
        }
        #endregion

        #region Event handlers
        public event EventHandler<FormatChangedEventArgs> FormatChanged;
        #endregion

        #region Constructors
        public FormatModel()
        {
            SetDefaultFormatting();
        }

        public FormatModel(string family, int size, string style, string weight, string color)
        {
            _fontFamily = family;
            _size = size;
            _fontStyle = style;
            _fontWeight = weight;
            _color = color;
            _charOffset = 1;
        }

        //arr[n]: Style Weight CharOffset Family Size Color
        public FormatModel(string style, string weight, int charoff, string family, int size, string color)
        {
            _fontFamily = family;
            _size = size;
            _fontStyle = style;
            _fontWeight = weight;
            _color = color;
            _charOffset = charoff;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Setting the default formatting.
        /// </summary>
        public void SetDefaultFormatting()
        {
            _size = 14;
            _fontWeight = "Normal";
            _fontFamily = "Courier new";
            _fontStyle = "Normal";
            _color = "Red";
            _charOffset = 1;
        }
        #endregion

        #region Event triggers
        /// <summary>
        /// Event for format changing.
        /// </summary>
        private void OnFormatChanged()
        {
            if (FormatChanged != null)
                FormatChanged(this, new FormatChangedEventArgs(_fontStyle, _fontWeight, _fontFamily, _size, _color));
        }
        #endregion
    }
}
