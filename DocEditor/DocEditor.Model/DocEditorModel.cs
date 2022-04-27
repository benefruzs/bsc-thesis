using System;
using System.Collections.Generic;
using System.IO;

namespace DocEditor.Model
{
    public enum Alignment { Left, Right, Center, Justified }
    public class DocEditorModel : ObservableObject
    {
        #region Private fields and Public properties
        public FormatModel FormatText;
        public MarginModel Margin;
        public Alignment align;
        public Selection select;
        
        private string _text;
        public string Text
        {
            get { return _text; }
            set { OnPropertyChanged(ref _text, value); }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { OnPropertyChanged(ref _filePath, value); }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { OnPropertyChanged(ref _fileName, value); }
        }

        private List<double> _fontSizes;
        public List<double> FontSizes
        {
            get { return _fontSizes; }
            set { _fontSizes = value; } 
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////
        /// </summary>
        private List<string> _fontFamilies;
        private List<string> FontFamilies
        {
            get { return _fontFamilies; }
            set { _fontFamilies = value; }
        }

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
        public DocEditorModel()
        {
            FormatText = new FormatModel();
            _fontSizes = new List<double>();
            for (double i = 2; i < 80; i += 2)
            {
                _fontSizes.Add(i);
            }

            align = Alignment.Left;
            select = new Selection();
            Margin = new MarginModel();

            _fileName = "Untitled";
        }
        #endregion

        #region Private methods

        #endregion

        #region Public methods
        public void setDefaultFormatting()
        {
            FormatText.SetDefaultFormatting();
            align = Alignment.Center;
            Margin.setDefaultMargin();
        }
        //save

        //load
        public DocEditorModel Load(MemoryStream stream)
        {
            return null;
        }
        #endregion
    }
}
