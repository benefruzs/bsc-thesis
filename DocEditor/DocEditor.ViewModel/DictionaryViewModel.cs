using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocEditor.Model;
using DocEditor.Parser;

namespace DocEditor.ViewModel
{
    public class DictionaryViewModel : ViewModelBase
    {
        #region Private fields and Public properties
        private DocEditorModel _model;
        private ParserMain _parser;

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if(value != null)
                {
                    _fileName = value;
                    OnPropertyChanged("FileName");
                }
            }
        }

        private List<DictClass> _dictElements;
        public List<DictClass> DictionaryElements
        {
            get { return _dictElements; }
            set
            {
                if (value != _dictElements){
                    _dictElements = value;
                    OnPropertyChanged("DictionaryElements");
                }
            }
        }

        private DictClass _selectedDictElement;
        public DictClass SelectedDictElement
        {
            get { return _selectedDictElement; }
            set
            {
                if (value != _selectedDictElement)
                {
                    //_model.FormatText.Family = value;
                    _selectedDictElement = value;
                    OnPropertyChanged("SelectedDictElement");
                }
            }
        }

        private string _dictString;
        public string DictString
        {
            get { return _dictString; }
            set
            {
                if (value != null)
                {
                    _dictString = value;
                    OnPropertyChanged("DictString");
                }
            }
        }


        public DelegateCommand AddToDictionaryCommand { get; private set; }

        public DelegateCommand SetDictFileNameCommand {get; private set;}

        public DelegateCommand SelectedDictElementChangedCommand { get; private set; }

        public DelegateCommand RemoveCommand { get; private set; }

        public ObservableCollection<DictionaryElements> DictElements { get; set; }
        #endregion

        #region Events
        public event EventHandler AddToDictionary;
        public event EventHandler SetDictFileName;
        public event EventHandler SelectedDictElementChanged;
        public event EventHandler RemoveEvent;
        #endregion

        #region Constructors
        public DictionaryViewModel(DocEditorModel model, ParserMain parser)
        {
            _model = model;
            _parser = parser;

            AddToDictionaryCommand = new DelegateCommand(param => OnAddToDict());
            SetDictFileNameCommand = new DelegateCommand(param => OnSetFileName());
            SelectedDictElementChangedCommand = new DelegateCommand(param => OnSetSelectDictElement());
            RemoveCommand = new DelegateCommand(param => OnRemove());

            DictElements = new ObservableCollection<DictionaryElements>();

            _dictElements = new List<DictClass>();
        }
        #endregion

        #region Methods
        public void addDictElements(DictClass dc)
        {
            _dictElements.Add(dc);
            OnPropertyChanged("DictionaryElements");
        }

        public void updateDictList()
        {

            if (_parser.Dict != null)
            {
                _dictElements = new List<DictClass>();
                foreach (var f in _parser.Dict)
                {
                    _dictElements.Add(f);
                }
                OnPropertyChanged("DictionaryElements");
            }
        }

        //addnew 
        public void DisplayText(FormatModel[] fm)
        {
            if (_selectedDictElement != null)
            {
                int len = _selectedDictElement.Str.Length;
                //FormatModel[] fm = _parser.getFormatting(_selectedDictElement);
                string bLAlignment = "Center";
                double top = 10;

                DictElements.Clear();

                for(int i=0; i<len; i++)
                {
                    switch (fm[i].CharOffset)
                    {
                        case 1:
                            bLAlignment = "Center";
                            top = 10;
                            break;
                        case -2:
                            bLAlignment = "Subscript";
                            top = 20;
                            break;
                        case 2:
                            bLAlignment = "Superscript";
                            top = 10;
                            break;
                    }
                    DictElements.Add(new DictionaryElements
                    {
                        RunText = _selectedDictElement.Str[i].ToString(),
                        DefaultSize = 20*fm[i].CharOffset,
                        DefaultFamily = fm[i].Family,
                        DefaultStyle = fm[i].Style,
                        DefaultWeight = fm[i].Weight,
                        DefaultColor = fm[i].Color,
                        BLineAlign = bLAlignment,
                        Top = top
                    });
                }
            }
        }

        #endregion

        #region Event methods
        private void OnAddToDict()
        {
            if (AddToDictionary != null)
                AddToDictionary(this, EventArgs.Empty);
        }

        private void OnSetFileName()
        {
            if (SetDictFileName != null)
                SetDictFileName(this, EventArgs.Empty);
        }

        private void OnSetSelectDictElement()
        {
            if (SelectedDictElementChanged != null)
                SelectedDictElementChanged(this, EventArgs.Empty);
        }

        private void OnRemove()
        {
            if (RemoveEvent != null)
                RemoveEvent(this, EventArgs.Empty);
        }
        #endregion
    }
}
