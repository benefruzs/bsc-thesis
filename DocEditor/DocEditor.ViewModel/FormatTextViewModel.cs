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

        private string _sFontSize;
        public string SFontSize
        { 
            get { return _sFontSize; } 
            set 
            {
                if (value != _sFontSize)
                {
                    //_model.FormatText.Size = value;
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
                    //_model.FormatText.Family = value;
                    _sFontFamily = value;
                    OnPropertyChanged("SFontFamily");
                }
            }
        }

        public List<string> FontStyles { get; private set; }

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
        public DelegateCommand OpenBorderCommand { get; private set; }
        public DelegateCommand AddMoreFormattingCommand { get; private set; }
        public DelegateCommand AddMoreSpacingCommand { get; private set; }

        public DelegateCommand FontFamilyChangedCmd { get; private set; }
        public DelegateCommand FontSizeChangedCmd { get; private set; }

        public DelegateCommand SetBoldCommand { get; private set; }
        public DelegateCommand SetItalicCommand { get; private set; }
        public DelegateCommand SetUnderLineCommand { get; private set; }
        public DelegateCommand SetStrikeThroughCommand { get; private set; }
        public DelegateCommand SetSuperScriptCommand { get; private set; }
        public DelegateCommand SetSubScriptCommand { get; private set; }

        public DelegateCommand LeftAlignCommand { get; private set; }
        public DelegateCommand CenterAlignCommand { get; private set; }
        public DelegateCommand JustifyAlignCommand { get; private set; }
        public DelegateCommand RightAlignCommand { get; private set; }


        public event EventHandler OpenColorPicker;
        public event EventHandler OpenBorder;
        public event EventHandler AddMoreFormatting;
        public event EventHandler AddMoreSpacing;
        public event EventHandler FontFamilyChanged;
        public event EventHandler FontSizeChanged;
        public event EventHandler RTBFocus;
        public event EventHandler SetBold;
        public event EventHandler SetItalic;
        public event EventHandler SetUnderLine;
        public event EventHandler SetStrikeThrough;
        public event EventHandler SetSuperScript;
        public event EventHandler SetSubScript;
        public event EventHandler LeftAlign;
        public event EventHandler CenterAlign;
        public event EventHandler JustifyAlign;
        public event EventHandler RightAlign;
        #endregion

        #region Constructors
        public FormatTextViewModel(DocEditorModel model)
        {
            _model = model;

            OpenColorPickerCommand = new DelegateCommand(param => OnColorPicker());
            OpenBorderCommand = new DelegateCommand(param => OnOpenBorder());

            AddMoreFormattingCommand = new DelegateCommand(param => OnMoreFormatting());
            AddMoreSpacingCommand = new DelegateCommand(param => OnMoreSpacing());
            FontFamilyChangedCmd = new DelegateCommand(param => OnFontFamilyChanged());
            FontSizeChangedCmd = new DelegateCommand(param => OnFontSizeChanged());
            SetBoldCommand = new DelegateCommand(param => OnSetBold());
            SetItalicCommand = new DelegateCommand(param => OnSetItalic());
            SetUnderLineCommand = new DelegateCommand(param => OnSetUnderLine());
            SetStrikeThroughCommand = new DelegateCommand(param => OnSetStrikeThrough());
            SetSuperScriptCommand = new DelegateCommand(param => OnSetSuperScript());
            SetSubScriptCommand = new DelegateCommand(param => OnSetSubScript());
            LeftAlignCommand = new DelegateCommand(param => OnSetLeftAlign());
            CenterAlignCommand = new DelegateCommand(param => OnSetCenterAlign());
            JustifyAlignCommand = new DelegateCommand(param => OnSetJustifyAlign());
            RightAlignCommand = new DelegateCommand(param => OnSetRightAlign());

            FontSizes = new List<double>();
            FontStyles = new List<string>();
            //FormatModel x = _model.FormatText;
            _sFontSize = _model.FormatText.Size.ToString();
            _sFontFamily = _model.FormatText.Family;
            _sTextColor = _model.FormatText.Color;

            FontSizes = _model.FontSizes;
            FontStyles = _model.FontStyles.Keys.ToList<string>();
            Console.WriteLine();
        }
        #endregion

        #region Event methods
        private void OnSetBold()
        {
            if (SetBold != null)
                SetBold(this, EventArgs.Empty);
        }

        private void OnSetItalic()
        {
            if (SetItalic != null)
                SetItalic(this, EventArgs.Empty);
        }
        private void OnSetUnderLine()
        {
            if (SetUnderLine != null)
                SetUnderLine(this, EventArgs.Empty);
        }
        private void OnSetStrikeThrough()
        {
            if (SetStrikeThrough != null)
                SetStrikeThrough(this, EventArgs.Empty);
        }

        private void OnSetSuperScript()
        {
            if (SetSuperScript != null)
                SetSuperScript(this, EventArgs.Empty);
        }
        private void OnSetSubScript()
        {
            if (SetSubScript != null)
                SetSubScript(this, EventArgs.Empty);
        }

        private void OnSetLeftAlign()
        {
            if (LeftAlign != null)
                LeftAlign(this, EventArgs.Empty);
        }
        private void OnSetCenterAlign()
        {
            if (CenterAlign != null)
                CenterAlign(this, EventArgs.Empty);
        }
        private void OnSetJustifyAlign()
        {
            if (JustifyAlign != null)
                JustifyAlign(this, EventArgs.Empty);
        }
        private void OnSetRightAlign()
        {
            if (RightAlign != null)
                RightAlign(this, EventArgs.Empty);
        }

        private void OnColorPicker()
        {
            if (OpenColorPicker != null)
                OpenColorPicker(this, EventArgs.Empty);
        }

        private void OnOpenBorder()
        {
            if (OpenBorder != null)
                OpenBorder(this, EventArgs.Empty);
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
                FontFamilyChanged(this, EventArgs.Empty);
        }

        private void OnFontSizeChanged()
        {
            if (FontSizeChanged != null)
                FontSizeChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
