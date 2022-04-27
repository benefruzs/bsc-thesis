using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocEditor.Model;

namespace DocEditor.ViewModel
{
    public class InsertViewModel : ViewModelBase
    {
        #region Private fields and Public properties
        private DocEditorModel _model;
        #endregion

        #region Constructors
        public InsertViewModel(DocEditorModel model)
        {
            _model = model;
        }
        #endregion
    }
}
