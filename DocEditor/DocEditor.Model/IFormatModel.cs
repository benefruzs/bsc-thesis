using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Model
{
    /// <summary>
    /// The interface of the model for text formatting.
    /// </summary>
    public interface IFormatModel
    {
        /// <summary>
        /// Get and set the font style.
        /// </summary>
        string Style { get; set; }

        /// <summary>
        /// Get and set the font weight.
        /// </summary>
        string Weight { get; set; }

        /// <summary>
        /// Get and set the font family.
        /// </summary>
        string Family { get; set; }

        /// <summary>
        /// Get and set the font size.
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// Get and set the text decoration.
        /// </summary>
        string TextDecoration { get; set; }

        /// <summary>
        /// Setting the default formatting.
        /// </summary>
        void SetDefaultFormatting();

        ///<summary>
        ///The event for the formatting changing.
        ///</summary>
        event EventHandler<FormatChangedEventArgs> FormatChanged;
    }
}
