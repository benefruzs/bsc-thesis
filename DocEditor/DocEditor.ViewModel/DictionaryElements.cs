using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.ViewModel
{
    public class DictionaryElements : ViewModelBase
    {
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
        /// Top padding
        /// </summary>
        private double _top;
        public double Top
        {
            get { return _top; }
            set
            {
                if (_top != value)
                {
                    _top = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Baseline alignment
        /// </summary>
        private string _bLineAlign;
        public string BLineAlign
        {
            get { return _bLineAlign; }
            set
            {
                if (_bLineAlign != value)
                {
                    _bLineAlign = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The text for the run
        /// </summary>
        private string _runText;
        public string RunText
        {
            get { return _runText; }
            set
            {
                if (_runText != value)
                {
                    _runText = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
