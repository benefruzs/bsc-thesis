using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocEditor.Persistence;

namespace DocEditor.Model
{

    /// <summary>
    /// DocEditor model type
    /// </summary>
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
            set { if (value != _formatText) { _formatText = value; } }
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

        /// <summary>
        /// Page line height
        /// </summary>
        public double LineHeight;

        /// <summary>
        /// The currently selected text
        /// </summary>
        public Selection select;

        /// <summary>
        /// Selected text for formatting
        /// </summary>
        public SelectionAndFormat SelectionAndFormat;

        /// <summary>
        /// The list of the inserted images
        /// </summary>
        public List<ImageClass> InsertedImages;

        /// <summary>
        /// Property for the file with the RichTextBox content
        /// </summary>
        private string _rtfFile;
        public string RtfFile
        {
            get { return _rtfFile; }
            set { _rtfFile = value; }
        }

        /// <summary>
        /// Property for the dictionary file
        /// </summary>
        private string _jsonFile;
        public string JsonFile
        {
            get { return _jsonFile; }
            set { _jsonFile = value; }
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
        /// Dictionary for the format styles
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

        /// <summary>
        /// The file was saved after the last edit or not
        /// </summary>
        public bool IsFileSaved{ get; private set; }

        /// <summary>
        /// Selected text for parsing
        /// </summary>
        public Stwf SelectForParser;

        /// <summary>
        /// Selected text list for error correcting
        /// </summary>
        public List<Stwf> ListForErrorCorrect;

        private IDocEditorDataAccess _dataAccess;


        public event EventHandler FileLoaded;
        public event EventHandler FileSaved;
        public event EventHandler FileChanged;
        public event EventHandler ModelFormatChanged;

        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate the Model
        /// </summary>
        public DocEditorModel(IDocEditorDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            DocInitialization();
            CreateDefaultFontStyles();
            _margin = new MarginModel();
            _fileName = "Untitled";

            PageWidth = 595;
            PageHeight = PageWidth / 7 * 10;
            ActualPageHeight = PageHeight - (_margin.Bottom + _margin.Top);
           
            LineHeight = 10;        
            IsFileSaved = false;
        }
        #endregion

        #region Private methods
        private void DocInitialization()
        {
            _formatText = new FormatModel();
            _fontSizes = new List<double>();
            for (double i = 2; i < 80; i += 2)
            {
                _fontSizes.Add(i);
            }

            _align = Alignment.Left;
            select = new Selection();
            
            ListForErrorCorrect = new List<Stwf>();
            InsertedImages = new List<ImageClass>();
            SelectForParser = new Stwf();
            _fontStyles = new Dictionary<string, FormatModel>();

        }

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
            _margin.SetDefaultMargin();
        }

        /// <summary>
        /// Loading a file
        /// </summary>
        /// <param name="path">loaded file path</param>
        public async Task LoadFileAsync(String path)
        {
            DocInitialization();
            
            if (_dataAccess == null) return;
            string[] values = new string[10];
            values = await _dataAccess.LoadAsync(path);

            _rtfFile = values[0];
            _jsonFile = values[1];
            PageWidth = Int32.Parse(values[2]);
            PageHeight = Int32.Parse(values[3]);
            _margin.Left = double.Parse(values[4]);
            _margin.Right = double.Parse(values[5]);
            _margin.Bottom = double.Parse(values[6]);
            _margin.Top = double.Parse(values[7]);
            LineHeight = double.Parse(values[8]);

            string[] tmp = values[9].Split('|');
            tmp = tmp.SkipLast(1).ToArray();

            string[] ttmp = new string[6];
            foreach (var t in tmp)
            {
                ttmp = t.Split(',');
                //string family, int size, string style, string weight, string color
                
                LoadNewFormatStyle(ttmp[0], new FormatModel(ttmp[1], Int32.Parse(ttmp[2]), ttmp[3], ttmp[4], ttmp[5]));
            }

            
            OnModelFormatChanged();
            OnFileLoaded();
            IsFileSaved = true;
        }

        /// <summary>
        /// Saving the information to a file
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns></returns>
        public async Task SaveFileAsync(String path)
        {
            if (_dataAccess == null) return;
            string[] saveArr = new string[10];

            saveArr[0] = _rtfFile;
            saveArr[1] = _jsonFile;
            saveArr[2] = PageWidth.ToString();
            saveArr[3] = PageHeight.ToString();
            saveArr[4] = _margin.Left.ToString();
            saveArr[5] = _margin.Right.ToString();
            saveArr[6] = _margin.Top.ToString();
            saveArr[7] = _margin.Bottom.ToString();
            saveArr[8] = LineHeight.ToString();

            //format styles
            foreach(var f in FontStyles)
            {
                //string family, int size, string style, string weight, string color
                saveArr[9] += f.Key.ToString() + ',';
                saveArr[9] += f.Value.Family + ',';
                saveArr[9] += f.Value.Size.ToString() + ',';
                saveArr[9] += f.Value.Style + ',';
                saveArr[9] += f.Value.Weight + ',';
                saveArr[9] += f.Value.Color + ',';
                saveArr[9] += '|';
            }

            await _dataAccess.SaveAsync(path, saveArr);
            OnFileSaved();
            IsFileSaved = true;
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

        /// <summary>
        /// Adding loaded new item to the format style list
        /// </summary>
        /// <param name="name">Name of the format style</param>
        /// <param name="fm">Formatting info</param>
        public void LoadNewFormatStyle(string name, FormatModel fm)
        {
            _fontStyles.Add(name, fm);
        }


        /// <summary>
        /// Creating new selection for formatting a text
        /// </summary>
        public void NewSelectForFormatting(Selection sel, Alignment al, FormatModel[] fm)
        {
            SelectionAndFormat = new SelectionAndFormat(sel, al, fm);
        }

        /// <summary>
        /// Get the most frequent formatting styles for a word's character's formatting
        /// </summary>
        public void GetFormatting()
        {
            if (SelectionAndFormat != null)
            {
                int len = SelectionAndFormat.SelectedText.Format.Length;
                var styleFreq = new Dictionary<string, int>(len);
                var weightFreq = new Dictionary<string, int>(len);
                var familyFreq = new Dictionary<string, int>(len);
                var chOffsetFreq = new Dictionary<int, int>(len);
                var sizeFreq = new Dictionary<int, int>(len);
                var colorFreq = new Dictionary<string, int>();

                for (int i = 0; i < len; i++)
                {
                    if (SelectionAndFormat.SelectedText.Format[i] != null)
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
        /// <returns>The reversed string</returns>
        public string ReverseWord(string word)
        {
            
            char[] strToReverse = word.ToCharArray();
            word = "";
            for (int i = strToReverse.Length - 1; i >= 0; i--)
            {
                word += strToReverse[i];
            }
            word = word.Trim('\r');
            word = word.Trim('\n');
            
            return word;
        }

        /// <summary>
        /// Getting the name of the main file
        /// </summary>
        /// <param name="fn">File path</param>
        public void GetFileName(string fn)
        {
            string[] arr = fn.Split("\\");
            _fileName = arr[arr.Length - 1];
        }

        /// <summary>
        /// Increases the LineHeight property by 2 and returns the LineHeight
        /// </summary>
        public double LineHeightIncr()
        {
            if (LineHeight < 49)
            {
                LineHeight += 2;
            }
            return LineHeight;
        }

        /// <summary>
        /// Decreases the LineHeight property by 2 and returns the LineHeight
        /// </summary>
        public double LineHeightDecr()
        {
            if (LineHeight > 7)
            {
                LineHeight -= 2;
            }
            return LineHeight;
        }

        /// <summary>
        /// Sets the value of the top margin from the user margin input
        /// </summary>
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

        /// <summary>
        /// Sets the value of the bottom margin from the user margin input
        /// </summary>
        public double SetBottomMargin(double mrgn)
        {
            if (mrgn * 20 < 101)
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

        /// <summary>
        /// Sets the value of the left margin from the user margin input
        /// </summary>
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

        /// <summary>
        /// Sets the value of the right margin from the user margin input
        /// </summary>
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

        /// <summary>
        /// Returns the bottom margin value in a form that the user can easily understand
        /// </summary>
        public double GetBottomMargin()
        {
            return _margin.Bottom / 20;
        }

        /// <summary>
        /// Returns the top margin value in a form that the user can easily understand
        /// </summary>
        public double GetTopMargin()
        {
            return _margin.Top / 20;
        }

        /// <summary>
        /// Returns the left margin value in a form that the user can easily understand
        /// </summary>
        public double GetLeftMargin()
        {
            return _margin.Left / 20;
        }

        /// <summary>
        /// Returns the right margin value in a form that the user can easily understand
        /// </summary>
        public double GetRightMargin()
        {
            return _margin.Right / 20;
        }

        /// <summary>
        /// Sets the double parameter value to all margins 
        /// </summary>
        /// <param name="mrgn">The value for the margins</param>
        public void SetAllMargins(double mrgn)
        {
            if (mrgn * 20 < 101)
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

        /// <summary>
        /// The file was modified after the last save
        /// </summary>
        public void SetFalseFileSaved()
        {
            IsFileSaved = false;
            OnFileChanged();
        }

        #endregion

        /// <summary>
        /// Event trigger for loading a file
        /// </summary>
        private void OnFileLoaded()
        {
            FileLoaded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event trigger for saving a file
        /// </summary>
        private void OnFileSaved()
        {
            FileSaved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event trigger for changes in the file
        /// </summary>
        private void OnFileChanged()
        {
            FileChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event trigger for format change
        /// </summary>
        private void OnModelFormatChanged()
        {
            ModelFormatChanged?.Invoke(this, EventArgs.Empty);
        }


    }
}
