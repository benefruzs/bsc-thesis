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
                    OnPropertyChanged(nameof(FileName));
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
                    OnPropertyChanged(nameof(DictionaryElements));
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
                    _selectedDictElement = value;
                    OnPropertyChanged(nameof(SelectedDictElement));
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
                    OnPropertyChanged(nameof(DictString));
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
        public void AddDictElements(DictClass dc)
        {
            _dictElements.Add(dc);
            OnPropertyChanged(nameof(DictionaryElements));
        }

        public void UpdateDictList()
        {

            if (_parser.Dict != null)
            {
                _dictElements = new List<DictClass>();
                foreach (var f in _parser.Dict)
                {
                    _dictElements.Add(f);
                }
                OnPropertyChanged(nameof(DictionaryElements));
            }
        }

        //addnew 
        public void DisplayText(FormatModel[] fm)
        {
            if (_selectedDictElement != null)
            {
                int len = _selectedDictElement.Str.Length;
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
                            bLAlignment = "TextBottom";
                            top = 20;
                            break;
                        case 2:
                            bLAlignment = "TextTop";
                            top = 10;
                            break;
                    }
                    DictElements.Add(new DictionaryElements
                    {
                        RunText = _selectedDictElement.Str[i].ToString(),
                        DefaultSize = Math.Abs(20/fm[i].CharOffset),
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

        public void DeletePreview()
        {
            DictElements.Clear();
        }

        #endregion

        #region Event methods
        private void OnAddToDict()
        {
            AddToDictionary?.Invoke(this, EventArgs.Empty);
        }

        private void OnSetFileName()
        {
            SetDictFileName?.Invoke(this, EventArgs.Empty);
        }

        private void OnSetSelectDictElement()
        {
            SelectedDictElementChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnRemove()
        {
            RemoveEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
