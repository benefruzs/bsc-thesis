using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Model
{
    public class ImageClass
    {
        #region Public properties and private fields
        private double _height;
        public double Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                }
            }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _width = value;
                }
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        private int _position;
        public int Position
        {
            get { return _position; }
            set
            {
                if (_position != value) 
                { _position = value; }
            }
        }
        #endregion

        #region Constructors
        public ImageClass(int ptr, string filePath, double h, double w)
        {
            _position = ptr;
            _imagePath = filePath;
            _height = h;
            _width = w;
        }
        #endregion
    }
}
