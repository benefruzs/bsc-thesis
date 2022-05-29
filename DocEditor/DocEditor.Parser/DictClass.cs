using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Parser
{
    /// <summary>
    /// Dictionary element type
    /// </summary>
    public class DictClass
    {
        #region Private fields and Public properties
        /// <summary>
        /// The word
        /// </summary>
        private string _string;
        public string Str
        {
            get { return _string; }
            set
            {
                if (_string != value) { _string = value; }
            }
        }

        /// <summary>
        /// The word's formatting for all characters
        /// </summary>
        private string[] _formatting;
        public string[] Formatting
        {
            get { return _formatting; }
            set
            {
                if (_formatting != value) { _formatting = value; }
            }
        }

        /// <summary>
        /// The element's frequency
        /// </summary>
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
        /// <summary>
        /// Creating a DictClass instance
        /// </summary>
        /// <param name="str"></param>
        /// <param name="format"></param>
        /// <param name="freq"></param>
        public DictClass(string str, string[] format, int freq)
        {
            _string = str;
            _formatting = format;
            _frequency = freq;
        }

        /// <summary>
        /// Creating a DictClass instance
        /// </summary>
        public DictClass()
        {

        }
        #endregion
    }
}
