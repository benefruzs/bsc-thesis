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
        /// <summary>
        /// Left page margin
        /// </summary>
        private double _left;
        public double Left
        {
            get { return _left; }
            set { if (value != _left) { _left = value; } }
        }

        /// <summary>
        /// Right page margin
        /// </summary>
        private double _right;
        public double Right
        {
            get { return _right; }
            set { if (value != _right) { _right = value; } }
        }

        /// <summary>
        /// Top page margin
        /// </summary>
        private double _top;
        public double Top
        {
            get { return _top; }
            set { if (value != _top) { _top = value; } }
        }

        /// <summary>
        /// Bottom page margin
        /// </summary>
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
            SetDefaultMargin();
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Setting the default page margin
        /// </summary>
        public void SetDefaultMargin()
        {
            _top = _bottom = _left = _right = 50;
        }

        /// <summary>
        /// Setting the page margins
        /// </summary>
        /// <param name="top">Top page margin</param>
        /// <param name="bottom">Bottom page margin</param>
        /// <param name="right">Right page margin</param>
        /// <param name="left">Left page margin</param>
        public void SetMargin(double top, double bottom, double right, double left)
        {
            _top = top;
            _bottom = bottom;
            _right = right;
            _left = left;
        }
        #endregion
    }
}
