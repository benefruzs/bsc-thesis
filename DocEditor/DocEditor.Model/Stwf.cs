using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Model
{
    //Text selection class for the parser
    public class Stwf //Selected Text With Formatting
    {
        #region Private fields and Public properties
        //the string itself
        private Selection _selectedText;
        public Selection SelectedText
        {
            get { return _selectedText; }
            set { if (_selectedText != value) { _selectedText = value; } }
        }

        //fromatting
        private FormatModel[] _textFormatting; //for every charachter of the string a formatting

        public FormatModel[] Format
        {
            get { return _textFormatting; }
            set { if (_textFormatting != value) { _textFormatting = value; } }
        }

        #endregion

        #region Constructors

        public Stwf()
        {
            _selectedText = new Selection();
            _textFormatting = null;
        }

        public Stwf(Selection sel, FormatModel[] fm)
        {
            _selectedText = sel;
            _textFormatting = fm;
        }

        #endregion
    }
}
