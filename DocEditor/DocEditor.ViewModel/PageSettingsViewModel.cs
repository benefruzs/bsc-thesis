using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocEditor.Model;

namespace DocEditor.ViewModel
{
    public class PageSettingsViewModel : ViewModelBase
    {
        #region Private fields and Public properties
        private DocEditorModel _model;
        #endregion

        #region DelegateCommands and EventHandlers
        public DelegateCommand MorePageSettingsCommand { get; private set; }
        
        public event EventHandler MorePageSettings;
        #endregion

        #region Constructors
        public PageSettingsViewModel(DocEditorModel model)
        {
            _model = model;

            MorePageSettingsCommand = new DelegateCommand(param => OnMorePageSettings());
        }
        #endregion

        #region Event methods
        private void OnMorePageSettings()
        {
            if (MorePageSettings != null)
                MorePageSettings(this, EventArgs.Empty);
        }
        #endregion
    }
}
