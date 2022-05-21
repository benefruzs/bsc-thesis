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

        private double _rightMargin;
        public double RightMargin
        {
            get { return _rightMargin; }
            set
            {
                if(value != _rightMargin)
                {
                    _rightMargin = value;
                    OnPropertyChanged(nameof(RightMargin));
                }
            }
        }
        private double _leftMargin;
        public double LeftMargin
        {
            get { return _leftMargin; }
            set
            {
                if (value != _leftMargin)
                {
                    _leftMargin = value;
                    OnPropertyChanged(nameof(LeftMargin));
                }
            }
        }
        private double _topMargin;
        public double TopMargin
        {
            get { return _topMargin; }
            set
            {
                if (value != _topMargin)
                {
                    _topMargin = value;
                    OnPropertyChanged(nameof(TopMargin));
                }
            }
        }
        private double _bottomMargin;
        public double BottomMargin
        {
            get { return _bottomMargin; }
            set
            {
                if (value != _bottomMargin)
                {
                    _bottomMargin = value;
                    OnPropertyChanged(nameof(BottomMargin));
                }
            }
        }

        #endregion

        #region DelegateCommands and EventHandlers
        public DelegateCommand MorePageSettingsCommand { get; private set; }

        public DelegateCommand SetTopMarginCommand { get; private set; }
        public DelegateCommand SetBottomMarginCommand { get; private set; }
        public DelegateCommand SetLeftMarginCommand { get; private set; }
        public DelegateCommand SetRightMarginCommand { get; private set; }

        public DelegateCommand Set05MarginCommand { get; private set; }
        public DelegateCommand Set15MarginCommand { get; private set; }
        public DelegateCommand Set25MarginCommand { get; private set; }
        public DelegateCommand Set35MarginCommand { get; private set; }
        public DelegateCommand ClosePageSettingsCommand { get; private set; }
        public DelegateCommand OkPageSettingsCommand { get; private set; }




    public event EventHandler MorePageSettings;

        public event EventHandler SetTopMargin;
        public event EventHandler SetBottomMargin;
        public event EventHandler SetLeftMargin;
        public event EventHandler SetRightMargin;

        public event EventHandler Set05Margin;
        public event EventHandler Set15Margin;
        public event EventHandler Set25Margin;
        public event EventHandler Set35Margin;

        public event EventHandler ClosePageSettings;
        public event EventHandler OkPageSettings;

        #endregion

        #region Constructors
        public PageSettingsViewModel(DocEditorModel model)
        {
            _model = model;

            MorePageSettingsCommand = new DelegateCommand(param => OnMorePageSettings());
            SetTopMarginCommand = new DelegateCommand(param => OnSetTopMargin());
            SetBottomMarginCommand = new DelegateCommand(param => OnSetBottomMargin());
            SetLeftMarginCommand = new DelegateCommand(param => OnSetLeftMargin());
            SetRightMarginCommand = new DelegateCommand(param => OnSetRightMargin());

            Set05MarginCommand = new DelegateCommand(param => OnSet05Margin());
            Set15MarginCommand = new DelegateCommand(param => OnSet15Margin());
            Set25MarginCommand = new DelegateCommand(param => OnSet25Margin());
            Set35MarginCommand = new DelegateCommand(param => OnSet35Margin());

            ClosePageSettingsCommand = new DelegateCommand(param => OnClosePageSettings());
            OkPageSettingsCommand = new DelegateCommand(param => OnOkPageSettings());

            _bottomMargin = _model.GetBottomMargin();
            _topMargin = _model.GetTopMargin();
            _leftMargin = _model.GetLeftMargin();
            _rightMargin = _model.GetRightMargin();
        }
        #endregion

        #region Public methods
        public void UpdateMargins()
        {
            _bottomMargin = _model.GetBottomMargin();
            _topMargin = _model.GetTopMargin();
            _leftMargin = _model.GetLeftMargin();
            _rightMargin = _model.GetRightMargin();
            OnPropertyChanged(nameof(BottomMargin));
            OnPropertyChanged(nameof(TopMargin));
            OnPropertyChanged(nameof(LeftMargin));
            OnPropertyChanged(nameof(RightMargin));
        }
        #endregion

        #region Event methods
        private void OnMorePageSettings()
        {
            MorePageSettings?.Invoke(this, EventArgs.Empty);
        }

        private void OnSetTopMargin()
        {
            SetTopMargin?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetBottomMargin()
        {
            SetBottomMargin?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetLeftMargin()
        {
            SetLeftMargin?.Invoke(this, EventArgs.Empty);
        }
        private void OnSetRightMargin()
        {
            SetRightMargin?.Invoke(this, EventArgs.Empty);
        }
        private void OnSet05Margin()
        {
            Set05Margin?.Invoke(this, EventArgs.Empty);
        }
        private void OnSet15Margin()
        {
            Set15Margin?.Invoke(this, EventArgs.Empty);
        }
        private void OnSet25Margin()
        {
            Set25Margin?.Invoke(this, EventArgs.Empty);
        }
        private void OnSet35Margin()
        {
            Set35Margin?.Invoke(this, EventArgs.Empty);
        }

        private void OnClosePageSettings()
        {
            ClosePageSettings?.Invoke(this, EventArgs.Empty);
        }

        private void OnOkPageSettings()
        {
            OkPageSettings?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
