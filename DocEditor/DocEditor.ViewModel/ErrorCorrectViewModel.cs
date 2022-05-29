using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DocEditor.Model;
using DocEditor.Parser;

namespace DocEditor.ViewModel
{
    public class ErrorCorrectViewModel : ViewModelBase
    {
        #region Private fields and public properties
        private DocEditorModel _model;
        private ParserMain _parser;

        /// <summary>
        /// The list of the dictionary elements
        /// </summary>
        private List<DictClass> _dictElements;
        public List<DictClass> PossibleDictionaryElements
        {
            get { return _dictElements; }
            set
            {
                if (value != _dictElements)
                {
                    _dictElements = value;
                    OnPropertyChanged(nameof(PossibleDictionaryElements));
                }
            }
        }

        /// <summary>
        /// Selected list element
        /// </summary>
        private DictClass _selectedDictElement;
        public DictClass SelectedPossibleDictElement
        {
            get { return _selectedDictElement; }
            set
            {
                if (value != _selectedDictElement)
                {
                    _selectedDictElement = value;
                    OnPropertyChanged(nameof(SelectedPossibleDictElement));
                }
            }
        }

        /// <summary>
        /// The string of a list element
        /// </summary>
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

        public ObservableCollection<DictionaryElements> PossibleDictElements { get; set; }

        public DelegateCommand SelectedPossibleDictElementChangedCommand { get; private set; }
        public DelegateCommand NoThanksCommand { get; private set; }
        public DelegateCommand OkayCommand { get; private set; }

        public event EventHandler SelectedPossibleDictElementChanged;
        public event EventHandler NoThanksEvent;
        public event EventHandler OkayEvent;
        #endregion

        #region Constructors
        public ErrorCorrectViewModel(DocEditorModel model, ParserMain parser)
        {
            _model = model;
            _parser = parser;

            SelectedPossibleDictElementChangedCommand = new DelegateCommand(param => OnSelected());
            NoThanksCommand = new DelegateCommand(param => OnNoThanks());
            OkayCommand = new DelegateCommand(param => OnOkay());

            PossibleDictElements = new ObservableCollection<DictionaryElements>();

            _dictElements = new List<DictClass>();
        }
        #endregion

        #region Private and public methods
 
        /// <summary>
        /// Update the dictionary list
        /// </summary>
        /// <param name="lst">The list to display</param>
        public void UpdateDictList(List<DictClass> lst)
        {
            if (lst != null)
            {
                _dictElements = new List<DictClass>();
                foreach (var f in lst)
                {
                    _dictElements.Add(f);
                }
                OnPropertyChanged(nameof(PossibleDictionaryElements));
            }
        }

        /// <summary>
        /// Display preview of the selected list element
        /// </summary>
        /// <param name="fm">The formatting</param>
        public void DisplayText(FormatModel[] fm)
        {
            if (_selectedDictElement != null)
            {
                int len = _selectedDictElement.Str.Length;
                //FormatModel[] fm = _parser.getFormatting(_selectedDictElement);
                string bLAlignment = "Center";
                double top = 10;

                PossibleDictElements.Clear();

                for (int i = 0; i < len; i++)
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
                    PossibleDictElements.Add(new DictionaryElements
                    {
                        RunText = _selectedDictElement.Str[i].ToString(),
                        DefaultSize = 20 / Math.Abs(fm[i].CharOffset),
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

        #region Event handlers
        private void OnSelected()
        {
            SelectedPossibleDictElementChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnNoThanks()
        {
            NoThanksEvent?.Invoke(this, EventArgs.Empty);
        }

        private void OnOkay()
        {
            OkayEvent?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
