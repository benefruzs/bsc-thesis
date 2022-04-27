using System;
using System.Windows;
using DocEditor.Model;

namespace DocEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Private members
        private DocEditorModel _model;
        private ViewModelBase _currentView;
        #endregion

        #region Public properties
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
        public DelegateCommand ExitAppCommand { get; private set; }
        public DelegateCommand MinimizeAppCommand { get; private set; }
        public DelegateCommand MaximizeAppCommand { get; private set; }

        public DelegateCommand OpenHomeWindowCommand { get; private set; }
        public DelegateCommand CloseHomeWindowCommand { get; private set; }
        public DelegateCommand MinimizeHomeCommand { get; private set; }
        public DelegateCommand MaximizeHomeCommand { get; private set; }

        public DelegateCommand OpenFormatTextCommand { get; private set; }
        public DelegateCommand OpenInsertCommand { get; private set; }
        public DelegateCommand OpenDictCommand { get; private set; }
        public DelegateCommand OpenEditPageCommand { get; private set; }
        public DelegateCommand OpenDictionaryHelpCmd { get; private set; }
        #endregion

        #region Events
        public event EventHandler OpenHome;
        public event EventHandler CloseHome;
        public event EventHandler MinHome;
        public event EventHandler MaxHome;

        public event EventHandler ExitApp;
        public event EventHandler MinApp;
        public event EventHandler MaxApp;

        public event EventHandler OpenFormatText;
        public event EventHandler OpenInsert;
        public event EventHandler OpenDict;
        public event EventHandler OpenEditPage;

        public event EventHandler OpenDictHelp;
        #endregion

        #region Constructors
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

            OpenFormatTextCommand = new DelegateCommand(param => OnFormatText());
            OpenDictCommand = new DelegateCommand(param => OnDict());
            OpenEditPageCommand = new DelegateCommand(param => OnEditPage());
            OpenInsertCommand = new DelegateCommand(param => OnInsert());

            OpenDictionaryHelpCmd = new DelegateCommand(param => OnOpenDictionaryHelp());

            PageMargins = new Thickness();

            DocName = _model.FileName;
            OnPropertyChanged("DocName");

            setDefaultFormatRtb();
            
        }
        #endregion

        #region private methods

        private void setDefaultFormatRtb()
        {
            _model.setDefaultFormatting();
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

            PageMargins = new Thickness(_model.Margin.Left, _model.Margin.Top, _model.Margin.Right, _model.Margin.Bottom);
            OnPropertyChanged("PageMargins");

            DefAlignment = _model.align.ToString();
            OnPropertyChanged("DefAlignment");
        }

        #endregion



        #region Event methods
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
