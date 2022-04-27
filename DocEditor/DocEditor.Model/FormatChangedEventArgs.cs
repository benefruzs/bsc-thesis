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
        public String Style { get; set; }

        /// <summary>
        /// Get and set the font weight.
        /// </summary>
        public String Weight { get; set; }

        /// <summary>
        /// Get and set the font family.
        /// </summary>
        public String Family { get; set; }

        /// <summary>
        /// Get and set the font size.
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// Get and set the font color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Event argumentum instantiation.
        /// </summary>
        /// <param name="s">FontStyle</param>
        /// <param name="w">FontWeight</param>
        /// <param name="f">FontFamily</param>
        /// <param name="size">FontSize</param>
        /// <param name="c">FontColor</param>
        public FormatChangedEventArgs(String s, String w, String f, double size, string c)
        {
            Style = s;
            Weight = w;
            Family = f;
            Size = size;
            Color = c;
        }
    }
}
