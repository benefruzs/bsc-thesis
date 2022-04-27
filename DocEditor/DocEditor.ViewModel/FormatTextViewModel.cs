using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DocEditor.Model;

namespace DocEditor.ViewModel
{
    public class FormatTextViewModel : ViewModelBase
    {
        #region Private fields and Public properties
        private DocEditorModel _model;

        public List<double> FontSizes { get; private set; }

        private int _sFontSize;
        public int SFontSize 
        { 
            get { return _sFontSize; } 
            set 
            {
                if (value != _sFontSize)
                {
                    _model.FormatText.Size = value;
                    _sFontSize = value;
                    OnPropertyChanged("SFontSize");
                }
            } 
        }

        private string _sFontFamily;
        public String SFontFamily
        {
            get { return _sFontFamily; }
            set
            {
                if (value != _sFontFamily)
                {
                    _model.FormatText.Family = value;
                    _sFontFamily = value;
                    OnPropertyChanged("SFontFamily");
                }
            }
        }

        private string _sTextColor;
        public string STextColor
        {
            get { return _sTextColor; }
            set
            {
                if (value != _sTextColor)
                {
                    _sTextColor = value;
                    OnPropertyChanged("STextColor");
                }
            }
        }
        #endregion

        #region DelegateCommands and EventHandlers
        public DelegateCommand OpenColorPickerCommand { get; private set; }
        public DelegateCommand AddMoreFormattingCommand { get; private set; }
        public DelegateCommand AddMoreSpacingCommand { get; private set; }

        public DelegateCommand FontFamilyChangedCmd { get; private set; }
        public DelegateCommand FontSizeChangedCmd { get; private set; }

        public event EventHandler OpenColorPicker;
        public event EventHandler AddMoreFormatting;
        public event EventHandler AddMoreSpacing;
        public event EventHandler<SelectionChangedEventArgs> FontFamilyChanged;
        public event EventHandler<TextChangedEventArgs> FontSizeChanged;
        public event EventHandler RTBFocus;
        #endregion

        #region Constructors
        public FormatTextViewModel(DocEditorModel model)
        {
            _model = model;

            OpenColorPickerCommand = new DelegateCommand(param => OnColorPicker());
            AddMoreFormattingCommand = new DelegateCommand(param => OnMoreFormatting());
            AddMoreSpacingCommand = new DelegateCommand(param => OnMoreSpacing());
            FontFamilyChangedCmd = new DelegateCommand(param => OnFontFamilyChanged());
            FontSizeChangedCmd = new DelegateCommand(param => OnFontSizeChanged());

            FontSizes = new List<double>();
            //FormatModel x = _model.FormatText;
            _sFontSize = _model.FormatText.Size;
            _sFontFamily = _model.FormatText.Family;
            _sTextColor = _model.FormatText.Color;

            FontSizes = _model.FontSizes;
        }
        #endregion

        #region Event methods
        private void OnColorPicker()
        {
            if (OpenColorPicker != null)
                OpenColorPicker(this, EventArgs.Empty);
        }

        private void OnMoreFormatting()
        {
            if (AddMoreFormatting != null)
                AddMoreFormatting(this, EventArgs.Empty);
        }

        private void OnMoreSpacing()
        {
            if (AddMoreSpacing != null)
                AddMoreSpacing(this, EventArgs.Empty);
        }

        private void OnFontFamilyChanged()
        {
            if (FontFamilyChanged != null)
                FontFamilyChanged(this, (SelectionChangedEventArgs)EventArgs.Empty);
        }

        private void OnFontSizeChanged()
        {
            if (FontSizeChanged != null)
                FontSizeChanged(this, (TextChangedEventArgs)EventArgs.Empty);
        }
        #endregion
    }
}
