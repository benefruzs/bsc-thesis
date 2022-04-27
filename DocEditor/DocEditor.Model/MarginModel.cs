using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Model
{
    public class MarginModel
    {
        #region Private fields and Public properties
        private double _left;
        public double Left
        {
            get { return _left; }
            set { if (value != _left) { _left = value; } }
        }

        private double _right;
        public double Right
        {
            get { return _right; }
            set { if (value != _right) { _right = value; } }
        }

        private double _top;
        public double Top
        {
            get { return _top; }
            set { if (value != _top) { _top = value; } }
        }

        private double _bottom;
        public double Bottom
        {
            get { return _bottom; }
            set { if (value != _bottom) { _bottom = value; } }
        }
        #endregion

        #region Constructors
        public MarginModel()
        {
            setDefaultMargin();
        }
        #endregion

        #region Private methods
        public void setDefaultMargin()
        {
            _top = _bottom = _left = _right = 50;
        }
        #endregion

        #region Public methods
        public void setMargin(double top, double bottom, double right, double left)
        {
            _top = top;
            _bottom = bottom;
            _right = right;
            _left = left;
        }
        #endregion
    }
}
