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
        private String _text;

        //fromatting
        private FormatModel[] _textFormatting; //for every charachter of the string a formatting

        public FormatModel[] Format
        {
            get { return _textFormatting; }
            set { if (_textFormatting != value) { _textFormatting = value; } }
        }

        public string Text
        {
            get { return _text; }
            set { if (_text != value) { _text = value; } }
        }
        #endregion

        #region Constructors

        #endregion
    }
}
