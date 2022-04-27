using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Model
{
    public class Selection
    {
        /// <summary>
        /// Class for selecting texts.
        /// </summary>
        #region Private fields and Public properties
        private int _startPointer;
        public int StartPointer
        {
            get { return _startPointer; }
            set 
            { 
                if (_startPointer != value) { _startPointer = value; } 
            }
        }

        private int _endPointer;
        public int EndPointer
        {
            get { return _endPointer; }
            set 
            {
                if (_endPointer != value) { _endPointer = value; } 
            }
        }

        private string _selectedString;
        public string SelectedString
        {
            get { return _selectedString; }
            set 
            {
                if (_selectedString != value) { _selectedString = value; }
            }
        }
        #endregion

        #region Constructors
        public Selection()
        {
            _startPointer = _endPointer = -1;
            _selectedString = null;
        }
        #endregion

        #region Public methods
        public void AddToFront_Selected(string add, int newStart)
        {
            _selectedString.Insert(0, add);
            _startPointer = newStart;
        }

        public void AddToEnd_Selected(string add, int newEnd)
        {
            _selectedString += add;
            _endPointer = newEnd;
        }

        public void AddToSelected(string add, int newStart, int newEnd)
        {
            if (newStart != _startPointer)
            {
                AddMore(add, newStart, newEnd, 0);
            }
            else
            {
                AddMore(add, newStart, newEnd, _selectedString.Length);
            }
        }

        public void DeleteSelection()
        {
            _startPointer = _endPointer = -1;
            _selectedString = null;
        }
        #endregion

        #region Private methods
        private void AddMore(string add, int newStart, int newEnd, int ind)
        {
            _selectedString.Insert(ind, add);
            _startPointer = newStart;
            _endPointer = newEnd;
        }
        #endregion

    }
}
