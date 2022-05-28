using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DocEditor.Model;

namespace DocEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Private members and Public properties
        /// <summary>
        /// Model class instance
        /// </summary>
        private DocEditorModel _model;

        /// <summary>
        /// The current view for the right side control
        /// </summary>
        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get { return _currentView; }
            set
            {
                if (value != _currentView)
                {
                    _currentView = value;
                    OnPropertyChanged(nameof(CurrentView));
                }
            }
        }


        public double LineHeightProp
        {
            get { return _model.LineHeight; }
            set
            {
                if (value != _model.LineHeight)
                {
                    _model.LineHeight = value;
                    OnPropertyChanged(nameof(LineHeightProp));
                }
            }
        }

        public ImageClass InsertedImage;

        //FontFamily="{Binding FF}" FontSize="{Binding SIZE}" FontStyle="{Binding FS}" FontWeight="{Binding FW}"
        public string FF { get; set; }
        public int SIZE { get; set; }
        public string FS { get; set; }
        public string FW { get; set; }
        public string FC { get; set; }
        public Thickness PageMargins { get; set; }
        public string DefAlignment { get; set; }

        private int _pageHeight;
        public int PageHeight {
            get { return _pageHeight = Convert.ToInt32(_model.Margin.Bottom) + 565; }
            set
            {
                _pageHeight = value;
                OnPropertyChanged();
            }
        }
        public int PageWidth {
            get { return _model.PageWidth; }
            set
            {
                _model.PageWidth = value;
                OnPropertyChanged();
            }
        }

        public string DocName { get; set; }


        /// <summary>
        /// Property for the SelectionAndFormat field from the model
        /// </summary>
        public SelectionAndFormat ModelSelectionAndFormat
        {
            get { return _model.SelectionAndFormat; }
            set
            {
                _model.SelectionAndFormat = value;
            }
        }

        /// <summary>
        /// Property for the Select field from the model
        /// </summary>
        public Selection ModelSelect
        {
            get { return _model.select; }
            set
            {
                _model.select = value;
            }
        }

        /// <summary>
        /// Property for the Align field from the model
        /// </summary>
        public Alignment ModelAlign
        {
            get { return _model.Align; }
            set
            {
                _model.Align = value;
            }
        }

        /// <summary>
        /// Property for the SelectForParser field from the model
        /// </summary>
        public Stwf ModelSelectForParser
        {
            get { return _model.SelectForParser; }
            set
            {
                _model.SelectForParser = value;
            }
        }

        /// <summary>
        /// Property for the FormatText field from the model
        /// </summary>
        public FormatModel ModelFormatText
        {
            get { return _model.FormatText; }
            set
            {
                _model.FormatText = value;
            }
        }


        /// <summary>
        /// Main window commands
        /// </summary>
        public DelegateCommand ExitAppCommand { get; private set; }
        public DelegateCommand MinimizeAppCommand { get; private set; }
        public DelegateCommand MaximizeAppCommand { get; private set; }

        /// <summary>
        /// Home window commands
        /// </summary>
        public DelegateCommand OpenHomeWindowCommand { get; private set; }
        public DelegateCommand CloseHomeWindowCommand { get; private set; }
        public DelegateCommand MinimizeHomeCommand { get; private set; }
        public DelegateCommand MaximizeHomeCommand { get; private set; }
        public DelegateCommand NewEmptyFileCommand { get; private set; }



        /// <summary>
        /// Right side control pages commands
        /// </summary>
        public DelegateCommand OpenFormatTextCommand { get; private set; }
        public DelegateCommand OpenInsertCommand { get; private set; }
        public DelegateCommand OpenDictCommand { get; private set; }
        public DelegateCommand OpenEditPageCommand { get; private set; }

        /// <summary>
        /// Menu commands
        /// </summary>
        public DelegateCommand OpenDictionaryHelpCmd { get; private set; }
        public DelegateCommand DeleteSelectionCommand { get; private set; }
        public DelegateCommand SelectAllTextCommand { get; private set; }
        public DelegateCommand NewPlainDocumentCommand { get; private set; }
        public DelegateCommand OpenDictionaryFileCommand { get; private set; }
        public DelegateCommand SaveDictionaryFileCommand { get; private set; }

        /// <summary>
        /// Commands for the RichTextBox
        /// </summary>
        public DelegateCommand SetSelectionCommand { get; private set; }
        public DelegateCommand AutoFormattingCommand {get; private set;}
        public DelegateCommand UpdateRTBCommand { get; private set; }
        public DelegateCommand NewParagraphCommand { get; private set; }


        public DelegateCommand UndoCommand { get; private set; }
        public DelegateCommand RedoCommand { get; private set; }
        public DelegateCommand PasteCommand { get; private set; }
        public DelegateCommand CutCommand { get; private set; }
        public DelegateCommand CopyCommand { get; private set; }
        public DelegateCommand ToUpperCommand { get; private set; }
        public DelegateCommand ToLowerCommand { get; private set; }

        public DelegateCommand OpenCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand SaveAsCommand { get; private set; }
        public DelegateCommand SaveToPdfCommand { get; private set; }
        public DelegateCommand OpenHelpCmd { get; private set; }

        public DelegateCommand HomeOpenCommand { get; private set; }
        public DelegateCommand HomeSaveCommand { get; private set; }
        public DelegateCommand HomeSaveAsCommand { get; private set; }

        public DelegateCommand NewNoteDictionaryCommand { get; private set; }
        public DelegateCommand NewMathDictionaryCommand { get; private set; }
        public DelegateCommand NewInfDictionaryCommand { get; private set; }

        #endregion

        #region Events
        public event EventHandler OpenHome;
        public event EventHandler CloseHome;
        public event EventHandler MinHome;
        public event EventHandler MaxHome;
        public event EventHandler NewEmptyFile;

        public event EventHandler ExitApp;
        public event EventHandler MinApp;
        public event EventHandler MaxApp;

        public event EventHandler OpenFormatText;
        public event EventHandler OpenInsert;
        public event EventHandler OpenDict;
        public event EventHandler OpenEditPage;

        public event EventHandler OpenDictHelp;
        public event EventHandler DeleteSelection;
        public event EventHandler SelectAllText;
        public event EventHandler NewPlainDocument;
        public event EventHandler OpenDictionaryFile;
        public event EventHandler SaveDictionaryFile;

        public event EventHandler SelectChanged;
        public event EventHandler AutoFormat;
        public event EventHandler NewParagraph;

        public event EventHandler UpdateRTB;
        public event EventHandler GotoFirstPage;
        public event EventHandler GotoLastPage;
        public event EventHandler GotoNextPage;
        public event EventHandler GotoPrevPage;

        public event EventHandler UndoEvent;
        public event EventHandler RedoEvent;
        public event EventHandler PasteEvent;
        public event EventHandler CutEvent;
        public event EventHandler CopyEvent;
        public event EventHandler ToUpperEvent;
        public event EventHandler ToLowerEvent;
        public event EventHandler OpenHelp;

        public event EventHandler OpenFile;
        public event EventHandler SaveFile;
        public event EventHandler SaveAsFile;
        public event EventHandler SaveToPdf;

        public event EventHandler HomeOpenFile;
        public event EventHandler HomeSaveFile;
        public event EventHandler HomeSaveAsFile;

        public event EventHandler NewNoteDictionary;
        public event EventHandler NewMathDictionary;
        public event EventHandler NewInfDictionary;

        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate the view model
        /// </summary>
        /// <param name="model">DocEditor model</param>
        public MainViewModel(DocEditorModel model)
        {
            _model = model;
            _model.FileChanged += new EventHandler(Model_FileChanged);
            _model.FileSaved += new EventHandler(Model_FileSaved);
            _model.FileLoaded += new EventHandler(Model_FileLoaded);
            _model.ModelFormatChanged += new EventHandler(Model_FormatChanged);

            ExitAppCommand = new DelegateCommand(param => OnExit());
            MinimizeAppCommand = new DelegateCommand(param => OnMinimize());
            MaximizeAppCommand = new DelegateCommand(param => OnMaximize());

            OpenHomeWindowCommand = new DelegateCommand(param => OnOpenHome());
            CloseHomeWindowCommand = new DelegateCommand(param => OnCloseHome());
            MinimizeHomeCommand = new DelegateCommand(param => OnMinimizeHome());
            MaximizeHomeCommand = new DelegateCommand(param => OnMaximizeHome());
            NewEmptyFileCommand = new DelegateCommand(param => OnNewEmptyFile());

            OpenFormatTextCommand = new DelegateCommand(param => OnFormatText());
            OpenDictCommand = new DelegateCommand(param => OnDict());
            OpenEditPageCommand = new DelegateCommand(param => OnEditPage());
            OpenInsertCommand = new DelegateCommand(param => OnInsert());

            OpenDictionaryHelpCmd = new DelegateCommand(param => OnOpenDictionaryHelp());
            DeleteSelectionCommand = new DelegateCommand(param => OnDeleteSelection());
            SelectAllTextCommand = new DelegateCommand(param => OnSelectAllText());
            NewPlainDocumentCommand = new DelegateCommand(param => OnNewPlainDocument());
            OpenDictionaryFileCommand = new DelegateCommand(param => OnOpenDictionaryFile());
            SaveDictionaryFileCommand = new DelegateCommand(param => OnSaveDictionaryFile());

            SetSelectionCommand = new DelegateCommand(param => OnSelectionChanged());
            AutoFormattingCommand = new DelegateCommand(param => OnKeyUp());
            NewParagraphCommand = new DelegateCommand(param => OnNewParagraph());

            UpdateRTBCommand = new DelegateCommand(param => OnTextChanged());

            UndoCommand = new DelegateCommand(param => OnUndo());
            RedoCommand = new DelegateCommand(param => OnRedo());
            PasteCommand = new DelegateCommand(param => OnPaste());
            CutCommand = new DelegateCommand(param => OnCut());
            CopyCommand = new DelegateCommand(param => OnCopy());
            ToUpperCommand = new DelegateCommand(param => OnToUpper());
            ToLowerCommand = new DelegateCommand(param => OnToLower());

            OpenHelpCmd = new DelegateCommand(param => OnOpenHelp());
            OpenCommand = new DelegateCommand(param => OnOpenFile());
            SaveCommand = new DelegateCommand(param => OnSaveFile());
            SaveAsCommand = new DelegateCommand(param => OnSaveAsFile());
            SaveToPdfCommand = new DelegateCommand(param => OnSaveToPdf());

            HomeOpenCommand = new DelegateCommand(param => OnHomeOpenFile());
            HomeSaveCommand = new DelegateCommand(param => OnHomeSaveFile());
            HomeSaveAsCommand = new DelegateCommand(param => OnHomeSaveAsFile());

            NewNoteDictionaryCommand = new DelegateCommand(param => OnNewNoteDictionary());
            NewMathDictionaryCommand = new DelegateCommand(param => OnNewMathDictionary());
            NewInfDictionaryCommand = new DelegateCommand(param => OnNewInfDictionary());

            PageMargins = new Thickness();

            SetDocumentName();
            SetDefaultFormatRtb();

            //7:10
            PageWidth = _model.PageWidth;
            //PageHeight = PageWidth / 7 * 10;
            _pageHeight = Convert.ToInt32(_model.Margin.Bottom) + 560;
            OnPropertyChanged(nameof(PageWidth));
            OnPropertyChanged(nameof(PageHeight));          

        }

        private void Model_FormatChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(LineHeightProp));
            OnPropertyChanged(nameof(PageWidth));
            OnPropertyChanged(nameof(PageHeight));

        }

        private void Model_FileLoaded(object sender, EventArgs e)
        {
            SetFormattingRTB();
            SetMarginsRTB();
            SetDocumentName();
        }

        private void Model_FileSaved(object sender, EventArgs e)
        {
            SetDocumentName();
        }

        private void Model_FileChanged(object sender, EventArgs e)
        {
            SetUnsavedFileName();
        }


        #endregion

        #region Public methods
        /// <summary>
        /// Sets the default formatting for the current document
        /// </summary>
        public void SetFormattingRTB()
        {
            FF = _model.FormatText.Family;
            SIZE = _model.FormatText.Size;
            FS = _model.FormatText.Style;
            FW = _model.FormatText.Weight;
            FC = _model.FormatText.Color;
            OnPropertyChanged(nameof(FF));
            OnPropertyChanged(nameof(SIZE));
            OnPropertyChanged(nameof(FS));
            OnPropertyChanged(nameof(FW));
            OnPropertyChanged(nameof(FC));
        }

        public void SetDocumentName()
        {
            DocName = _model.FileName;
            OnPropertyChanged(nameof(DocName));
        }

        public void SetUnsavedFileName()
        {
            DocName = _model.FileName + "*";
            OnPropertyChanged(nameof(DocName));
        }

        /// <summary>
        /// Sets the margins for the current document
        /// </summary>
        public void SetMarginsRTB()
        {
            PageMargins = new Thickness(_model.Margin.Left, _model.Margin.Top, _model.Margin.Right, _model.Margin.Bottom);
            OnPropertyChanged(nameof(PageMargins));
            OnPropertyChanged(nameof(PageHeight));
        }


        public void SetInsertedImage(int ptr, string path, double h, double w)
        {
            InsertedImage = new ImageClass(ptr, path, h, w);
        }

        public void AddToImageList()
        {
            _model.InsertedImages.Add(InsertedImage);
        }

        public void Model_AddNewFormatStyle()
        {
            _model.AddNewFormatStyle(ModelSelectionAndFormat.Formatting);
        }

        public void Model_GetFormatting()
        {
            _model.GetFormatting();
        }

        public string Model_ReverseWord(string wordToReverse)
        {
            return _model.ReverseWord(wordToReverse);
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Default formatting for new documents
        /// </summary>
        private void SetDefaultFormatRtb()
        {
            _model.SetDefaultFormatting();
            SetFormattingRTB();

            SetMarginsRTB();

            DefAlignment = _model.Align.ToString();
            OnPropertyChanged(nameof(DefAlignment));
        }

        #endregion


        #region Event methods
        private void OnNewNoteDictionary()
        {
            NewNoteDictionary?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewMathDictionary()
        {
            NewMathDictionary?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewInfDictionary()
        {
            NewInfDictionary?.Invoke(this, EventArgs.Empty);
        }
        private void OnHomeSaveAsFile()
        {
            HomeSaveAsFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnHomeSaveFile()
        {
            HomeSaveFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnHomeOpenFile()
        {
            HomeOpenFile?.Invoke(this, EventArgs.Empty);
        }
        private void OnSaveDictionaryFile()
        {
            SaveDictionaryFile?.Invoke(this, EventArgs.Empty);
        }
        private void OnSaveToPdf()
        {
            SaveToPdf?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveAsFile()
        {
            SaveAsFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveFile()
        {
            SaveFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnOpenFile()
        {
            OpenFile?.Invoke(this, EventArgs.Empty);
        }
        private void OnOpenHelp()
        {
            OpenHelp?.Invoke(this, EventArgs.Empty);
        }
        private void OnUndo()
        {
            UndoEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnRedo()
        {
            RedoEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnCut()
        {
            CutEvent?.Invoke(this, EventArgs.Empty);
        }
        private void OnCopy()
        {
            CopyEvent?.Invoke(this, EventArgs.Empty);
        }
        private void OnPaste()
        {
            PasteEvent?.Invoke(this, EventArgs.Empty);
        }
        private void OnToUpper()
        {
            ToUpperEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnToLower()
        {
            ToLowerEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectionChanged()
        {
            SelectChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnKeyUp()
        {
            AutoFormat?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewParagraph()
        {
            NewParagraph?.Invoke(this, EventArgs.Empty);
        }

        private void OnTextChanged()
        {
            UpdateRTB?.Invoke(this, EventArgs.Empty);
        }

        private void OnFormatText()
        {
            OpenFormatText?.Invoke(this, EventArgs.Empty);
        }
        private void OnDict()
        {
            OpenDict?.Invoke(this, EventArgs.Empty);
        }
        private void OnEditPage()
        {
            OpenEditPage?.Invoke(this, EventArgs.Empty);
        }
        private void OnInsert()
        {
            OpenInsert?.Invoke(this, EventArgs.Empty);
        }

        private void OnOpenDictionaryHelp()
        {
            OpenDictHelp?.Invoke(this, EventArgs.Empty);
        }

        private void OnDeleteSelection()
        {
            DeleteSelection?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectAllText()
        {
            SelectAllText?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewPlainDocument()
        {
            NewPlainDocument?.Invoke(this, EventArgs.Empty);
        }

        private void OnOpenDictionaryFile()
        {
            OpenDictionaryFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnOpenHome()
        {
            OpenHome?.Invoke(this, EventArgs.Empty);
        }
        private void OnCloseHome()
        {
            CloseHome?.Invoke(this, EventArgs.Empty);
        }
        private void OnMinimizeHome()
        {
            MinHome?.Invoke(this, EventArgs.Empty);
        }

        private void OnMaximizeHome()
        {
            MaxHome?.Invoke(this, EventArgs.Empty);
        }

        private void OnNewEmptyFile()
        {
            NewEmptyFile?.Invoke(this, EventArgs.Empty);
        }

        private void OnMinimize()
        {
            MinApp?.Invoke(this, EventArgs.Empty);
        }

        private void OnMaximize()
        {
            MaxApp?.Invoke(this, EventArgs.Empty);
        }

        private void OnExit()
        {
            ExitApp?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }

}
