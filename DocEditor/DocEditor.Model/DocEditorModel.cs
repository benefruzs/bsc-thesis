using System;
using System.Collections.Generic;
using System.IO;

namespace DocEditor.Model
{
    /// <summary>
    /// Enum type for paragraph alignment
    /// </summary>
    public enum Alignment { Left, Right, Center, Justify }
    public class DocEditorModel : ObservableObject
    {
        #region Private fields and Public properties
        /// <summary>
        /// The current default formatting for the new texts
        /// </summary>
        private FormatModel _formatText;
        public FormatModel FormatText 
        {
            get { return _formatText; }
            set { if(value != _formatText) { _formatText = value; } }
        }

        /// <summary>
        /// The margin property for all pages
        /// </summary>
        private MarginModel _margin;
        public MarginModel Margin
        {
            get { return _margin; }
            set { if (value != _margin) { _margin = value; } }
        }

        /// <summary>
        /// The current default alignment for the document
        /// </summary>
        private Alignment _align;
        public Alignment Align
        {
            get { return _align; }
            set { if (value != _align) { _align = value; } }
        }

        /// <summary>
        /// The currently selected text
        /// </summary>
        public Selection select;

        public SelectionAndFormat SelectionAndFormat;
        
        /// <summary>
        /// Property for the texts shoud stored in the model
        /// </summary>
        private string _text;
        public string Text
        {
            get { return _text; }
            set { OnPropertyChanged(ref _text, value); }
        }

        /// <summary>
        /// Property of the file path
        /// </summary>
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { OnPropertyChanged(ref _filePath, value); }
        }

        /// <summary>
        /// Property of the file name
        /// </summary>
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { OnPropertyChanged(ref _fileName, value); }
        }

        /// <summary>
        /// Font size list
        /// </summary>
        private List<double> _fontSizes;
        public List<double> FontSizes
        {
            get { return _fontSizes; }
            set { _fontSizes = value; } 
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        private Dictionary<string, FormatModel> _fontStyles;
        public Dictionary<string, FormatModel> FontStyles
        {
            get { return _fontStyles; }
            private set { _fontStyles = value; }
        }

        /// <summary>
        /// Empty file name or empty file path
        /// </summary>
        public bool isEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(FileName)) return true;
                return false;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate the Model
        /// </summary>
        public DocEditorModel()
        {
            _formatText = new FormatModel();
            _fontSizes = new List<double>();
            for (double i = 2; i < 80; i += 2)
            {
                _fontSizes.Add(i);
            }

            _align = Alignment.Left;
            select = new Selection();
            _margin = new MarginModel();

            _fileName = "Untitled";

            createDefaultFontStyles();
        }
        #endregion

        #region Private methods

        #endregion

        #region Public methods
        /// <summary>
        /// Setting the default foratting for the document startup
        /// </summary>
        public void setDefaultFormatting()
        {
            _formatText.SetDefaultFormatting();
            _align = Alignment.Center;
            _margin.setDefaultMargin();
        }
        
        //save

        /// <summary>
        /// Loading a file
        /// </summary>
        /// <param name="stream">memory stream</param>
        /// <returns>Model class intance</returns>
        public DocEditorModel Load(MemoryStream stream)
        {
            return null;
        }

        public void createDefaultFontStyles()
        {
            _fontStyles = new Dictionary<string, FormatModel>();
            FormatModel Style1 = new FormatModel("Arial", 12, "Normal", "Normal", "Black");
            _fontStyles.Add("Alap", Style1);
            FormatModel Style2 = new FormatModel("Arial", 32, "Normal", "Bold", "Black");
            _fontStyles.Add("Címek", Style2);
            FormatModel Style3 = new FormatModel("Arial", 18, "Normal", "Bold", "Skyblue");
            _fontStyles.Add("Alcímek", Style3);
        }

        public void newSelectForFormatting(Selection sel, Alignment al, FormatModel[] fm)
        {
            SelectionAndFormat = new SelectionAndFormat(sel, al, fm);
        }
        #endregion
    }
}
