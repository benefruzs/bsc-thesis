using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocEditor.Model;

namespace DocEditor.ViewModel
{
    public class DictionaryViewModel : ViewModelBase
    {
        #region Private fields and Public properties
        private DocEditorModel _model;

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

        public DelegateCommand AddToDictionaryCommand { get; private set; }

        public DelegateCommand SetDictFileNameCommand {get; private set;}
        #endregion

        #region Events
        public event EventHandler AddToDictionary;
        public event EventHandler SetDictFileName;
        #endregion

        #region Constructors
        public DictionaryViewModel(DocEditorModel model)
        {
            _model = model;

            AddToDictionaryCommand = new DelegateCommand(param => OnAddToDict());
            SetDictFileNameCommand = new DelegateCommand(param => OnSetFileName());
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
        #endregion
    }
}
