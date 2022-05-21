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

        private int _currentPageNumber;

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

        private double _lineHeightProp;
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

        //FontFamily="{Binding FF}" FontSize="{Binding SIZE}" FontStyle="{Binding FS}" FontWeight="{Binding FW}"
        public string FF { get; set; }
        public int SIZE { get; set; }
        public string FS { get; set; }
        public string FW { get; set; }
        public string FC { get; set; }
        public Thickness PageMargins { get; set; }
        public string DefAlignment { get; set; }
        public int PageHeight {
            get { return _model.PageHeight; }
            set { _model.PageHeight = value; OnPropertyChanged(); } }
        public int PageWidth {
            get { return _model.PageWidth; }
            set { _model.PageWidth = value; OnPropertyChanged(); } }

        public string DocName { get; set; }

        public int CurrentPageNumber { get; set; }
        public int AllPageNumbers { get; set; }

        /// <summary>
        /// RichTextBox list with the properties
        /// </summary>
        public ObservableCollection<DocPaper> DocPages { get; private set; }

        /// <summary>
        /// The size of the pages
        /// </summary>
        //public Int32 SizeX { get { return _model.SizeX; } }
        //public Int32 SizeY { get { return _model.SizeY; } }

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

        /// <summary>
        /// Commands for the RichTextBox
        /// </summary>
        public DelegateCommand SetSelectionCommand { get; private set; }
        public DelegateCommand AutoFormattingCommand {get; private set;}
        public DelegateCommand UpdateRTBCommand { get; private set; }
        public DelegateCommand NewParagraphCommand { get; private set; }

        /// <summary>
        /// Page navigation commands
        /// </summary>
        public DelegateCommand GotoFirstPageCommand { get; private set; }
        public DelegateCommand GotoLastPageCommand { get; private set; }
        public DelegateCommand GotoNextPageCommand { get; private set; }
        public DelegateCommand GotoPrevPageCommand { get; private set; }


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

        public event EventHandler SelectChanged;
        public event EventHandler AutoFormat;
        public event EventHandler NewParagraph;

        public event EventHandler UpdateRTB;
        public event EventHandler GotoFirstPage;
        public event EventHandler GotoLastPage;
        public event EventHandler GotoNextPage;
        public event EventHandler GotoPrevPage;

        #endregion

        #region Constructors
        /// <summary>
        /// Instantiate the view model
        /// </summary>
        /// <param name="model">DocEditor model</param>
        public MainViewModel(DocEditorModel model)
        {
            _model = model;

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

            SetSelectionCommand = new DelegateCommand(param => OnSelectionChanged());
            AutoFormattingCommand = new DelegateCommand(param => OnKeyUp());
            NewParagraphCommand = new DelegateCommand(param => OnNewParagraph());

            UpdateRTBCommand = new DelegateCommand(param => OnTextChanged());
            GotoFirstPageCommand = new DelegateCommand(param => OnGotoFirst());
            GotoLastPageCommand = new DelegateCommand(param => OnGotoLast());
            GotoNextPageCommand = new DelegateCommand(param => OnGotoNext());
            GotoPrevPageCommand = new DelegateCommand(param => OnGotoPrev());

            DocPages = new ObservableCollection<DocPaper>();

            PageMargins = new Thickness();

            _currentPageNumber = 0;

            setPageNumbers(1, 1);

            DocName = _model.FileName;
            OnPropertyChanged(nameof(DocName));

            setDefaultFormatRtb();

            //7:10
            PageWidth = _model.PageWidth;
            PageHeight = PageWidth / 7 * 10;
            OnPropertyChanged(nameof(PageWidth));
            OnPropertyChanged(nameof(PageHeight));

            

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

        /// <summary>
        /// Sets the margins for the current document
        /// </summary>
        public void SetMarginsRTB()
        {
            PageMargins = new Thickness(_model.Margin.Left, _model.Margin.Top, _model.Margin.Right, _model.Margin.Bottom);
            OnPropertyChanged(nameof(PageMargins));
        }


        public void setPageNumbers(int all, int current)
        {
            CurrentPageNumber = current;
            AllPageNumbers = all;
            OnPropertyChanged(nameof(CurrentPageNumber));
            OnPropertyChanged(nameof(AllPageNumbers));
        }

        //public void addFirstPage()
        //{
        //    DocPages.Clear();
        //    DocPages.Add(new DocPaper
        //    {
        //        SizeX = _model.SizeX,
        //        SizeY = _model.SizeY,
        //        DefaultWeight = _model.FormatText.Weight,
        //        DefaultColor = _model.FormatText.Color,
        //        DefaultFamily = _model.FormatText.Family,
        //        DefaultSize = _model.FormatText.Size,
        //        LeftMargin = _model.Margin.Left,
        //        RightMargin = _model.Margin.Right,
        //        TopMargin = _model.Margin.Top,
        //        BottomMargin = _model.Margin.Bottom,
        //        PageNumber = 0,
        //        isMainPage = true
        //    });
        //}

        /// <summary>
        /// Adding new page to the collection
        /// </summary>
        /// <param name="ind">The page index of the previous page</param>
        //public void AddNewPage(int ind)
        //{
        //    //a docpages utolsója lekérdez, az ismainpaget hamisra kell állítani
        //    DocPages.Last().isMainPage = false;
        //    // új page a docpageshez, modellbõl kell a margin, és a default formázást beállítani
        //    DocPages.Add(new DocPaper
        //    {
        //        SizeX = _model.SizeX,
        //        SizeY = _model.SizeY,
        //        DefaultWeight = _model.FormatText.Weight,
        //        DefaultColor = _model.FormatText.Color,
        //        DefaultFamily = _model.FormatText.Family,
        //        DefaultSize = _model.FormatText.Size,
        //        LeftMargin = _model.Margin.Left,
        //        RightMargin = _model.Margin.Right,
        //        TopMargin = _model.Margin.Top,
        //        BottomMargin = _model.Margin.Bottom,
        //        PageNumber = ind+1,
        //        isMainPage = true
        //    });
            
        //}

        #endregion

        #region Private methods

        /// <summary>
        /// Default formatting for new documents
        /// </summary>
        private void setDefaultFormatRtb()
        {
            _model.SetDefaultFormatting();
            SetFormattingRTB();

            SetMarginsRTB();

            DefAlignment = _model.Align.ToString();
            OnPropertyChanged(nameof(DefAlignment));
        }
       
        #endregion


        #region Event methods
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

        private void OnGotoFirst()
        {
            GotoFirstPage?.Invoke(this, EventArgs.Empty);
        }
        private void OnGotoLast()
        {
            GotoLastPage?.Invoke(this, EventArgs.Empty);
        }
        private void OnGotoNext()
        {
            GotoNextPage?.Invoke(this, EventArgs.Empty);
        }
        private void OnGotoPrev()
        {
            GotoPrevPage?.Invoke(this, EventArgs.Empty);
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
