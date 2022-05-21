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
                    OnPropertyChanged(nameof(SFontSize));
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
                    OnPropertyChanged(nameof(SFontFamily));
                }
            }
        }

        private string _selectedStyle;
        public string SelectedStyle
        {
            get { return _selectedStyle; }
            set
            {
                if (value != _selectedStyle)
                {
                    _selectedStyle = value;
                    OnPropertyChanged(nameof(SelectedStyle));
                }
            }
        }

        private List<string> _fontStyles;
        public List<string> FontStyles
        {
            get { return _fontStyles; }
            set
            {
                if (value != _fontStyles)
                {
                    _fontStyles = value;
                    OnPropertyChanged(nameof(FontStyles));
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
                    OnPropertyChanged(nameof(STextColor));
                }
            }
        }

        private double _lineHeightProp;
        public double LineHeightProp
        {
            get { return _lineHeightProp; }
            set
            {
                if (value != _lineHeightProp)
                {
                    _lineHeightProp = value;
                    OnPropertyChanged(nameof(LineHeightProp));
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

        public DelegateCommand StyleChangedCommand { get; private set; }
        public DelegateCommand AddNewStyleCommand { get; private set; }
        public DelegateCommand DeleteFormattingCommand { get; private set; }

        public DelegateCommand AddLineHeightCommand { get; private set; }
        public DelegateCommand LessLineHeightCommand { get; private set; }


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

        public event EventHandler StyleChanged;
        public event EventHandler NewStyleAdded;
        public event EventHandler DeleteFormatting;
        public event EventHandler AddLineHeight;
        public event EventHandler LessLineHeight;
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

            StyleChangedCommand = new DelegateCommand(param => OnStyleSelect());
            AddNewStyleCommand = new DelegateCommand(param => OnNewStyle());
            DeleteFormattingCommand = new DelegateCommand(param => OnDeleteFormatting());
            AddLineHeightCommand = new DelegateCommand(param => OnAddLineHeight());
            LessLineHeightCommand = new DelegateCommand(param => OnLessLineHeight());

            FontSizes = new List<double>();
            FontStyles = new List<string>();
            //FormatModel x = _model.FormatText;
            _sFontSize = _model.FormatText.Size.ToString();
            _sFontFamily = _model.FormatText.Family;
            _sTextColor = _model.FormatText.Color;
            _lineHeightProp = _model.LineHeight;

            FontSizes = _model.FontSizes;
            UpdateStyleList();
            Console.WriteLine();
        }
        #endregion

        #region Public methods
        public void UpdateStyleList()
        {
            System.Diagnostics.Debug.WriteLine(_model.SelectionAndFormat);
            _fontStyles = new List<string>();
            foreach (var f in _model.FontStyles)
            {
                _fontStyles.Add(f.Key);
            }

            //_fontStyles = _model.FontStyles.Keys.ToList<string>();
            OnPropertyChanged("FontStyles");

        }

        public void IncreaseLineHeight()
        {
            _lineHeightProp = _model.LineHeightIncr();
            OnPropertyChanged(nameof(LineHeightProp));
        }

        public void DecreaseLineHeight()
        {
            _lineHeightProp = _model.LineHeightDecr();
            OnPropertyChanged(nameof(LineHeightProp));
        }
        #endregion

        #region Event methods
        private void OnSetBold()
        {
            SetBold?.Invoke(this, EventArgs.Empty);
        }

        private void OnSetItalic()
        {
            SetItalic?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetUnderLine()
        {
            SetUnderLine?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetStrikeThrough()
        {
            SetStrikeThrough?.Invoke(this, EventArgs.Empty);
        }

        private void OnSetSuperScript()
        {
            SetSuperScript?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetSubScript()
        {
            SetSubScript?.Invoke(this, EventArgs.Empty);
        }

        private void OnSetLeftAlign()
        {
            LeftAlign?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetCenterAlign()
        {
            CenterAlign?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetJustifyAlign()
        {
            JustifyAlign?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetRightAlign()
        {
            RightAlign?.Invoke(this, EventArgs.Empty);
        }

        private void OnColorPicker()
        {
            OpenColorPicker?.Invoke(this, EventArgs.Empty);
        }

        private void OnOpenBorder()
        {
            OpenBorder?.Invoke(this, EventArgs.Empty);
        }

        private void OnMoreFormatting()
        {
            AddMoreFormatting?.Invoke(this, EventArgs.Empty);
        }

        private void OnMoreSpacing()
        {
            AddMoreSpacing?.Invoke(this, EventArgs.Empty);
        }

        private void OnFontFamilyChanged()
        {
            FontFamilyChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnFontSizeChanged()
        {
            FontSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnStyleSelect()
        {
            StyleChanged?.Invoke(this, EventArgs.Empty);
        }
        private void OnNewStyle()
        {
            NewStyleAdded?.Invoke(this, EventArgs.Empty);
        }
        private void OnDeleteFormatting()
        {
            DeleteFormatting?.Invoke(this, EventArgs.Empty);
        }

        private void OnAddLineHeight()
        {
            AddLineHeight?.Invoke(this, EventArgs.Empty);
        }
        private void OnLessLineHeight()
        {
            LessLineHeight?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
