using System;
using System.Windows;
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

        //FontFamily="{Binding FF}" FontSize="{Binding SIZE}" FontStyle="{Binding FS}" FontWeight="{Binding FW}"
        public string FF { get; set; }
        public int SIZE { get; set; }
        public string FS { get; set; }
        public string FW { get; set; }
        public string FC { get; set; }
        public Thickness PageMargins { get; set; }
        public string DefAlignment { get; set; }

        public string DocName { get; set; }

        /// <summary>
        /// 
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

        public DelegateCommand SetSelectionCommand { get; private set; }


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

        public event EventHandler SelectChanged;
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

            SetSelectionCommand = new DelegateCommand(param => OnSelectionChanged());

            PageMargins = new Thickness();

            DocName = _model.FileName;
            OnPropertyChanged("DocName");

            setDefaultFormatRtb();
            
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the default formatting for the current document
        /// </summary>
        public void setFormattingRTB()
        {
            FF = _model.FormatText.Family;
            SIZE = _model.FormatText.Size;
            FS = _model.FormatText.Style;
            FW = _model.FormatText.Weight;
            FC = _model.FormatText.Color;
            OnPropertyChanged("FF");
            OnPropertyChanged("SIZE");
            OnPropertyChanged("FS");
            OnPropertyChanged("FW");
            OnPropertyChanged("FC");
        }

        /// <summary>
        /// Sets the margins for the current document
        /// </summary>
        public void setMarginsRTB()
        {
            PageMargins = new Thickness(_model.Margin.Left, _model.Margin.Top, _model.Margin.Right, _model.Margin.Bottom);
            OnPropertyChanged("PageMargins");
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Default formatting for new documents
        /// </summary>
        private void setDefaultFormatRtb()
        {
            _model.setDefaultFormatting();
            setFormattingRTB();

            setMarginsRTB();

            DefAlignment = _model.Align.ToString();
            OnPropertyChanged("DefAlignment");
        }
       
        #endregion


        #region Event methods
        private void OnSelectionChanged()
        {
            if (SelectChanged != null)
                SelectChanged(this, EventArgs.Empty);
        }
        private void OnFormatText()
        {
            if (OpenFormatText != null)
                OpenFormatText(this, EventArgs.Empty);
        }
        private void OnDict()
        {
            if (OpenDict != null)
                OpenDict(this, EventArgs.Empty);
        }
        private void OnEditPage()
        {
            if (OpenEditPage != null)
                OpenEditPage(this, EventArgs.Empty);
        }
        private void OnInsert()
        {
            if (OpenInsert != null)
                OpenInsert(this, EventArgs.Empty);
        }

        private void OnOpenDictionaryHelp()
        {
            if (OpenDictHelp != null)
                OpenDictHelp(this, EventArgs.Empty);
        }

        private void OnDeleteSelection()
        {
            if (DeleteSelection != null)
                DeleteSelection(this, EventArgs.Empty);
        }

        private void OnSelectAllText()
        {
            if (SelectAllText != null)
                SelectAllText(this, EventArgs.Empty);
        }

        private void OnNewPlainDocument()
        {
            if (NewPlainDocument != null)
                NewPlainDocument(this, EventArgs.Empty);
        }

        private void OnOpenHome()
        {
            if (OpenHome != null)
                OpenHome(this, EventArgs.Empty);
        }
        private void OnCloseHome()
        {
            if (CloseHome != null)
                CloseHome(this, EventArgs.Empty);
        }
        private void OnMinimizeHome()
        {
            if (MinHome != null)
                MinHome(this, EventArgs.Empty);
        }

        private void OnMaximizeHome()
        {
            if (MaxHome != null)
                MaxHome(this, EventArgs.Empty);
        }

        private void OnNewEmptyFile()
        {
            if (NewEmptyFile != null)
                NewEmptyFile(this, EventArgs.Empty);
        }

        private void OnMinimize()
        {
            if (MinApp != null)
                MinApp(this, EventArgs.Empty);
        }

        private void OnMaximize()
        {
            if (MaxApp != null)
                MaxApp(this, EventArgs.Empty);
        }

        private void OnExit()
        {
            if (ExitApp != null)
                ExitApp(this, EventArgs.Empty);
        }
        #endregion
    }

}
