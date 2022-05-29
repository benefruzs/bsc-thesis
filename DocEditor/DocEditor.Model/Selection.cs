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

        /// <summary>
        /// The start of the selection
        /// </summary>
        private int _startPointer;
        public int StartPointer
        {
            get { return _startPointer; }
            set 
            { 
                if (_startPointer != value) { _startPointer = value; } 
            }
        }

        /// <summary>
        /// The end of the selection
        /// </summary>
        private int _endPointer;
        public int EndPointer
        {
            get { return _endPointer; }
            set 
            {
                if (_endPointer != value) { _endPointer = value; } 
            }
        }

        /// <summary>
        /// The selected string
        /// </summary>
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

        public Selection(int st, int nd, string txt)
        {
            _startPointer = st;
            _endPointer = nd;
            _selectedString = txt;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adding more selected text to the beginning
        /// </summary>
        /// <param name="add">the selected string</param>
        /// <param name="newStart">the new start pointer</param>
        public void AddToFront_Selected(string add, int newStart)
        {
            _selectedString = _selectedString.Insert(0, add);
            _startPointer = newStart;
        }

        /// <summary>
        /// Adding more selected text to the end
        /// </summary>
        /// <param name="add">the selected string</param>
        /// <param name="newEnd">the new end pointer</param>
        public void AddToEnd_Selected(string add, int newEnd)
        {
            _selectedString += add;
            _endPointer = newEnd;
        }

        /// <summary>
        /// Update the selection
        /// </summary>
        /// <param name="add">the selected string</param>
        /// <param name="newStart">the new start pointer</param>
        /// <param name="newEnd"> the new end pointer</param>
        public void AddToSelected(string add, int newStart, int newEnd)
        {
            //adding more selected string
            UpdateSelected(add, newStart, newEnd);
        }

        /// <summary>
        /// Delete all selection
        /// </summary>
        public void DeleteSelection()
        {
            _startPointer = _endPointer = -1;
            _selectedString = null;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Private method for updating the selected text
        /// </summary>
        /// <param name="add">the selected text</param>
        /// <param name="newStart">new start pointer</param>
        /// <param name="newEnd">new end pointer</param>
        private void UpdateSelected(string add, int newStart, int newEnd)
        {
            _selectedString = add;
            _startPointer = newStart;
            _endPointer = newEnd;
        }
        #endregion

    }
}
