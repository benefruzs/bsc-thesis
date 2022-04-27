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
        //Italic, Normal, Oblique
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

        //black, bold, demiBold, extraBlack, extraLight, heavy, light, medium, normal, regular, semiBold, thin, ultraBlack, ultraBold, ultraLight 
        private string _fontWeight;
        public string Weight
        {
            get { return _fontWeight; }
            set {
                if (_fontWeight != value) { _fontWeight = value; OnFormatChanged(); }
            }
        }

        private string _fontFamily;
        public string Family
        {
            get { return _fontFamily; }
            set {
                if (_fontFamily != value) { _fontFamily = value; OnFormatChanged(); }
            }
        }

        private int _charOffset; //-10 subscript, 10 superscript, 0 normal
        public int CharOffset
        {
            get { return _charOffset; }
            set { 
                if (_charOffset != value) { _charOffset = value; OnFormatChanged(); } 
            }
        }

        private int _size;
        public int Size
        {
            get { return _size; }
            set {
                if (_size != value) { _size = value; OnFormatChanged(); }
            }
        }
        private string _color;
        public string Color
        {
            get { return _color; }
            set
            {
                if(_color != value) { _color = value; OnFormatChanged(); }
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
        #endregion

        #region Public methods
        /// <summary>
        /// Setting the default formatting.
        /// </summary>
        public void SetDefaultFormatting()
        {
            _size = 48;
            _fontWeight = "Normal";
            _fontFamily = "Courier new";
            _fontStyle = "Normal";
            _color = "Red";
            _charOffset = 0;
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
