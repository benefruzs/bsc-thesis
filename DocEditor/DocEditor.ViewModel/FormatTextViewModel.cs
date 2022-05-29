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

        /// <summary>
        /// Font size list
        /// </summary>
        public List<double> FontSizes { get; private set; }

        /// <summary>
        /// The selected font size
        /// </summary>
        private string _sFontSize;
        public string SFontSize
        {
            get { return _sFontSize; }
            set
            {
                if (value != _sFontSize)
                {
                    _sFontSize = value;
                    OnPropertyChanged(nameof(SFontSize));
                }
            }
        }

        /// <summary>
        /// The selected font family
        /// </summary>
        private string _sFontFamily;
        public String SFontFamily
        {
            get { return _sFontFamily; }
            set
            {
                if (value != _sFontFamily)
                {
                    _sFontFamily = value;
                    OnPropertyChanged(nameof(SFontFamily));
                }
            }
        }

        /// <summary>
        /// The selected font style
        /// </summary>
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

        /// <summary>
        /// List of font styles
        /// </summary>
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

        /// <summary>
        /// Selected text color
        /// </summary>
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

        /// <summary>
        /// Line height
        /// </summary>
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
        public event EventHandler FontFamilyChanged;
        public event EventHandler FontSizeChanged;
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
            _model.ModelFormatChanged += new EventHandler(Model_FormatChanged);

            OpenColorPickerCommand = new DelegateCommand(param => OnColorPicker());
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
            _sFontSize = _model.FormatText.Size.ToString();
            _sFontFamily = _model.FormatText.Family;
            _sTextColor = _model.FormatText.Color;
            _lineHeightProp = _model.LineHeight;

            FontSizes = _model.FontSizes;
            UpdateStyleList();
            Console.WriteLine();
        }

        private void Model_FormatChanged(object sender, EventArgs e)
        {
            UpdateStyleList();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Update the list of format styles
        /// </summary>
        public void UpdateStyleList()
        {
            
            _fontStyles = new List<string>();
            foreach (var f in _model.FontStyles)
            {
                _fontStyles.Add(f.Key);
            }

            OnPropertyChanged(nameof(FontStyles));

        }

        /// <summary>
        /// Increase the line height
        /// </summary>
        public void IncreaseLineHeight()
        {
            _lineHeightProp = _model.LineHeightIncr();
            OnPropertyChanged(nameof(LineHeightProp));
        }

        /// <summary>
        /// Decrease the line height
        /// </summary>
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
