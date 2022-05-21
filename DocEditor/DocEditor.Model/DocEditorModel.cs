using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocEditor.Model
{
    /// <summary>
    /// Enum type for paragraph alignment
    /// </summary>
    public enum Alignment { Left, Right, Center, Justify }
    public class DocEditorModel
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
        /// The size of all pages
        /// </summary>
        public int PageHeight;
        public int PageWidth;
        public double ActualPageHeight;

        public double LineHeight;

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
            set { _text = value; }
        }

        /// <summary>
        /// Property of the file path
        /// </summary>
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// Property of the file name
        /// </summary>
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
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
            set { _fontStyles = value; }
        }

        /// <summary>
        /// Empty file name or empty file path
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(FileName)) return true;
                return false;
            }
        }

        public Stwf SelectForParser;
        public List<Stwf> ListForErrorCorrect;

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

            PageWidth = 595;
            PageHeight = PageWidth/7 * 10;

            ActualPageHeight = PageHeight - (_margin.Bottom + _margin.Top);

            CreateDefaultFontStyles();

            //selectForParser = new Stwf();
            ListForErrorCorrect = new List<Stwf>();

            LineHeight = 10;
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Create the default formatting list
        /// </summary>
        private void CreateDefaultFontStyles()
        {
            _fontStyles = new Dictionary<string, FormatModel>();
            FormatModel Style1 = new FormatModel("Arial", 12, "Normal", "Normal", "Black");
            _fontStyles.Add("Alap", Style1);
            FormatModel Style2 = new FormatModel("Arial", 32, "Normal", "Bold", "Black");
            _fontStyles.Add("Címek", Style2);
            FormatModel Style3 = new FormatModel("Arial", 18, "Normal", "Bold", "Skyblue");
            _fontStyles.Add("Alcímek", Style3);
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Setting the default foratting for the document startup
        /// </summary>
        public void SetDefaultFormatting()
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

        /// <summary>
        /// Add new element to the formatting style list
        /// </summary>
        /// <param name="fm">The new formatting</param>
        public void AddNewFormatStyle(FormatModel fm)
        {
            string styleName = "Stílus" + (_fontStyles.Count - 3).ToString();
            _fontStyles.Add(styleName, fm);
        }

        public void NewSelectForFormatting(Selection sel, Alignment al, FormatModel[] fm)
        {
            SelectionAndFormat = new SelectionAndFormat(sel, al, fm);
        }

        /// <summary>
        /// Get the most frequent formatting styles for a word's character's formatting
        /// </summary>
        public void GetFormatting()
        {
            if (SelectionAndFormat != null) {
                int len = SelectionAndFormat.SelectedText.Format.Length;
                var styleFreq = new Dictionary<string, int>(len);
                var weightFreq = new Dictionary<string, int>(len);
                var familyFreq = new Dictionary<string, int>(len);
                var chOffsetFreq = new Dictionary<int, int>(len);
                var sizeFreq = new Dictionary<int, int>(len);
                var colorFreq = new Dictionary<string, int>();

                for (int i = 0; i < len; i++)
                {
                    if (styleFreq.ContainsKey(SelectionAndFormat.SelectedText.Format[i].Style))
                    {
                        styleFreq[SelectionAndFormat.SelectedText.Format[i].Style]++;
                    }
                    else
                    {
                        styleFreq.Add(SelectionAndFormat.SelectedText.Format[i].Style, 1);
                    }

                    if (weightFreq.ContainsKey(SelectionAndFormat.SelectedText.Format[i].Weight))
                    {
                        weightFreq[SelectionAndFormat.SelectedText.Format[i].Weight]++;
                    }
                    else
                    {
                        weightFreq.Add(SelectionAndFormat.SelectedText.Format[i].Weight, 1);
                    }

                    if (familyFreq.ContainsKey(SelectionAndFormat.SelectedText.Format[i].Family))
                    {
                        familyFreq[SelectionAndFormat.SelectedText.Format[i].Family]++;
                    }
                    else
                    {
                        familyFreq.Add(SelectionAndFormat.SelectedText.Format[i].Family, 1);
                    }

                    if (chOffsetFreq.ContainsKey(SelectionAndFormat.SelectedText.Format[i].CharOffset))
                    {
                        chOffsetFreq[SelectionAndFormat.SelectedText.Format[i].CharOffset]++;
                    }
                    else
                    {
                        chOffsetFreq.Add(SelectionAndFormat.SelectedText.Format[i].CharOffset, 1);
                    }

                    if (sizeFreq.ContainsKey(SelectionAndFormat.SelectedText.Format[i].Size))
                    {
                        sizeFreq[SelectionAndFormat.SelectedText.Format[i].Size]++;
                    }
                    else
                    {
                        sizeFreq.Add(SelectionAndFormat.SelectedText.Format[i].Size, 1);
                    }

                    if (colorFreq.ContainsKey(SelectionAndFormat.SelectedText.Format[i].Color))
                    {
                        colorFreq[SelectionAndFormat.SelectedText.Format[i].Color]++;
                    }
                    else
                    {
                        colorFreq.Add(SelectionAndFormat.SelectedText.Format[i].Color, 1);
                    }
                }

                SelectionAndFormat.Formatting.Style = styleFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                SelectionAndFormat.Formatting.Weight = weightFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                SelectionAndFormat.Formatting.Family = familyFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                SelectionAndFormat.Formatting.CharOffset = chOffsetFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                SelectionAndFormat.Formatting.Size = sizeFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                SelectionAndFormat.Formatting.Color = colorFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }

        }

        /// <summary>
        /// Method to reverse a string
        /// </summary>
        /// <param name="word">The string</param>
        /// <returns></returns>
        public string ReverseWord(string word)
        {
            char[] strToReverse = word.ToCharArray();
            word = "";
            for(int i=strToReverse.Length-1; i >=0; i--)
            {
                word += strToReverse[i];
            }
            return word;
        }

        public double LineHeightIncr()
        {
            if(LineHeight < 49)
            {
                LineHeight += 2;
            }
            return LineHeight;
        }

        public double LineHeightDecr()
        {
            if (LineHeight > 7)
            {
                LineHeight -= 2;
            }
            return LineHeight;
        }

        public double SetTopMargin(double mrgn)
        {
            if (mrgn * 20 < 101)
            {
                if (mrgn < 0)
                {
                    _margin.Top = 0;
                }
                else
                {
                    _margin.Top = mrgn * 20;
                }
            }
            else
            {
                _margin.Top = 100;
            }
            return _margin.Top;
        }
        public double SetBottomMargin(double mrgn)
        {
            if(mrgn * 20 < 101)
            {
                if (mrgn < 0)
                {
                    _margin.Bottom = 0;
                }
                else
                {
                    _margin.Bottom = mrgn * 20;
                }
            }
            else
            {
                _margin.Bottom = 100;
            }
            return _margin.Bottom;
        }
        public double SetLeftMargin(double mrgn)
        {
            if (mrgn * 20 < 101)
            {
                if (mrgn < 0)
                {
                    _margin.Left = 0;
                }
                else
                {
                    _margin.Left = mrgn * 20;
                }
            }
            else
            {
                _margin.Left = 100;
            }
            return _margin.Left;
        }
        public double SetRightMargin(double mrgn)
        {
            if (mrgn * 20 < 101)
            {
                if (mrgn < 0)
                {
                    _margin.Right = 0;
                }
                else
                {
                    _margin.Right = mrgn * 20;
                }
            }
            else
            {
                _margin.Right = 100;
            }
            return _margin.Right;
        }
        public double GetBottomMargin()
        {
            return _margin.Bottom / 20;
        }
        public double GetTopMargin()
        {
            return _margin.Top / 20;
        }
        public double GetLeftMargin()
        {
            return _margin.Left / 20;
        }
        public double GetRightMargin()
        {
            return _margin.Right / 20;
        }

        public void SetAllMargins(double mrgn)
        {
            if(mrgn * 20 < 101)
            {
                if (mrgn < 0)
                {
                    _margin.Right = 0;
                    _margin.Left = 0;
                    _margin.Top = 0;
                    _margin.Bottom = 0;
                }
                else
                {
                    _margin.Right = mrgn * 20;
                    _margin.Left = mrgn * 20;
                    _margin.Top = mrgn * 20;
                    _margin.Bottom = mrgn * 20;
                }
            }
            else
            {
                _margin.Right = 100;
                _margin.Left = 100;
                _margin.Top = 100;
                _margin.Bottom = 100;
            }
        }

        #endregion
    }
}
