using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Parser
{
    public class DictClass
    {
        #region Private fields and Public properties
        private string _string;
        public string Str
        {
            get { return _string; }
            set
            {
                if (_string != value) { _string = value; }
            }
        }
        private string[] _formatting;
        public string[] Formatting
        {
            get { return _formatting; }
            set
            {
                if (_formatting != value) { _formatting = value; }
            }
        }
        private int _frequency;
        public int Frequency
        {
            get { return _frequency; }
            set
            {
                if (_frequency != value) { _frequency = value; }
            }
        }
        
        #endregion

        #region Constructors
        public DictClass(string str, string[] format, int freq)
        {
            _string = str;
            _formatting = format;
            _frequency = freq;
        }
        #endregion

        #region Public methods

        #endregion
    }
}
