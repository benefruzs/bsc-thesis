using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Model
{
    /// <summary>
    /// Class for formatting the selected text by the user
    /// </summary>
    public class SelectionAndFormat
    {
        #region Private fields and Public properties
        /// <summary>
        /// The selected text
        /// </summary>

        private Stwf _selectedText;
        public Stwf SelectedText
        {
            get { return _selectedText; }
            set { if (_selectedText != value) { _selectedText = value; } }
        }

        /// <summary>
        /// The formatting for the text selection
        /// </summary>
        private FormatModel _formatting;
        public FormatModel Formatting
        {
            get { return _formatting; }
            set { if(value != _formatting) { _formatting = value; } }
        }

        /// <summary>
        /// The alignment for the paragraph which contains the selected text
        /// </summary>
        private Alignment _align;
        public Alignment Align
        {
            get { return _align; }
            set { if (value != _align) { _align = value; } }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="_select">The selected text</param>
        /// <param name="_al">The current alignment for the paragraph</param>
        public SelectionAndFormat(Selection select, Alignment al, FormatModel[] fm)
        {
            _selectedText = new Stwf();
            _selectedText.SelectedText = select;
            _selectedText.Format = fm;
            _align = al;
            _formatting = new FormatModel();
        }

        public SelectionAndFormat(Selection select)
        {
            _selectedText = new Stwf(select, null);
            _formatting = new FormatModel();
            _align = Alignment.Left;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Method for deleting all the formatting
        /// </summary>
        public void DeleteAllFormatting()
        {
            _formatting.SetDefaultFormatting();
        }

        /// <summary>
        /// Method for setting the fontweight to bold
        /// </summary>
        public void SetBold()
        {
            _formatting.Weight = "Bold";
        }

        public void SetWeight(string weight)
        {
            _formatting.Weight = weight;
        }

        /// <summary>
        /// Style changing methods
        /// </summary>
        public void SetItalic()
        {
            _formatting.Style = "Italic";
        }

        public void SetOblique()
        {
            _formatting.Style = "Oblique";
        }

        /// <summary>
        /// Setting the charachter offSet
        /// </summary>
        public void SetSubscript()
        {
            _formatting.CharOffset = -2;
        }

        public void SetSuperscript()
        {
            _formatting.CharOffset = 2;
        }

        public void DeleteSubSuperscript()
        {
            _formatting.CharOffset = 1;
        }

        /// <summary>
        /// Delete the added fontstyle
        /// </summary>
        public void DeleteStyle()
        {
            _formatting.Style = "Normal";
        }

        /// <summary>
        /// Delete the added fontweight
        /// </summary>
        public void DeleteWeight()
        {
            _formatting.Weight = "Normal";
        }

        /// <summary>
        /// Change the fontsize
        /// </summary>
        /// <param name="si">new fontsize</param>
        public void ChangeSize(int si)
        {
            _formatting.Size = si;
        }

        /// <summary>
        /// Change the font color
        /// </summary>
        /// <param name="col">new color</param>
        public void ChangeColor(string col)
        {
            _formatting.Color = "#" + col;
        }

        /// <summary>
        /// Change the font family
        /// </summary>
        /// <param name="fam">new font family</param>
        public void ChangeFont(string fam)
        {
            _formatting.Family = fam;
        }

        /// <summary>
        /// Setting the alignment for the selected string to left alignment
        /// </summary>
        public void SetLeftAlignment()
        {
            _align = Alignment.Left;
        }

        /// <summary>
        /// Setting the alignment for the selected string to center alignment
        /// </summary>
        public void SetCenterAlignment()
        {
            _align = Alignment.Center;
        }

        /// <summary>
        /// Setting the alignment for the selected string to Justify alignment
        /// </summary>
        public void SetJustifyAlignment()
        {
            _align = Alignment.Justify;
        }

        /// <summary>
        /// Setting the alignment for the selected string to right alignment
        /// </summary>
        public void SetRightAlignment()
        {
            _align = Alignment.Right;
        }

        public void SetSelectedString(string str)
        {
            _selectedText.SelectedText.SelectedString = str;
            _selectedText.SelectedText.EndPointer = _selectedText.SelectedText.StartPointer + str.Length;
        }

        public string GetSelectedText()
        {
            return _selectedText.SelectedText.SelectedString;
        }

        public Selection GetSelection()
        {
            return _selectedText.SelectedText;
        }
        #endregion
    }
}
