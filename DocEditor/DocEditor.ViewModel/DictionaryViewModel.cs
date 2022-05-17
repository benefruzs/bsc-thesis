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

        public DelegateCommand AddToDictionaryCommand { get; private set;}
        #endregion

        #region Events
        public event EventHandler AddToDictionary;
        #endregion

        #region Constructors
        public DictionaryViewModel(DocEditorModel model)
        {
            _model = model;

            AddToDictionaryCommand = new DelegateCommand(param => OnAddToDict());
        }
        #endregion

        #region Event methods
        private void OnAddToDict()
        {
            if (AddToDictionary != null)
                AddToDictionary(this, EventArgs.Empty);
        }
        #endregion
    }
}
