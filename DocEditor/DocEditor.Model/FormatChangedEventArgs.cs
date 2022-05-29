using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Model
{
    public class FormatChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Get and set the font style.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Get and set the font weight.
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Get and set the font family.
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// Get and set the font size.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Get and set the font color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Get and set the text decoration.
        /// </summary>
        public string TextDecoration { get; set; }

        /// <summary>
        /// Event argumentum instantiation.
        /// </summary>
        /// <param name="s">FontStyle</param>
        /// <param name="w">FontWeight</param>
        /// <param name="f">FontFamily</param>
        /// <param name="size">FontSize</param>
        /// <param name="c">FontColor</param>
        public FormatChangedEventArgs(string s, string w, string f, double size, string c, string d)
        {
            Style = s;
            Weight = w;
            Family = f;
            Size = size;
            Color = c;
            TextDecoration = d;
        }
    }
}
