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

        /// <summary>
        /// The path of the inserted image
        /// </summary>
        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (value != _imagePath)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                } 
            }
        }

        /// <summary>
        /// The height of the inserted image
        /// </summary>
        private double _imageHeight;
        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                if(value != _imageHeight)
                {
                    _imageHeight = value;
                }
            }
        }

        /// <summary>
        /// The width of the inserted image
        /// </summary>
        private double _imageWidth;
        public double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                if (value != _imageWidth)
                {
                    _imageWidth = value;
                }
            }
        }

        public DelegateCommand InsertImageCommand { get; private set; }

        public event EventHandler InsertImageEvent;
        #endregion

        #region Constructors
        public InsertViewModel(DocEditorModel model)
        {
            _model = model;

            InsertImageCommand = new DelegateCommand(param => OnInsertImage());
        }
        #endregion

        #region Event handlers
        private void OnInsertImage()
        {
            InsertImageEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
