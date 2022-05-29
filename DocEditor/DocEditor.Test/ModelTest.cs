using System.Collections.Generic;
using DocEditor.Model;
using DocEditor.Parser;
using DocEditor.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DocEditor.Test
{
    [TestClass]
    public class ModelTest
    {
        #region Private fields
        private DocEditorModel _model;
        #endregion

        #region Initialization
        [TestInitialize]
        public void Initialize()
        {
            _model = new DocEditorModel(new DocEditorFileDataAccess());
            _model.select = new Selection(1, 5, "test selection");
            _model.SelectionAndFormat = new SelectionAndFormat(_model.select);
        }
        #endregion

        #region SelectionAndFormat tests
        [TestMethod]
        public void InitializeTest1()
        {
            SelectionAndFormat saf = new SelectionAndFormat(_model.select);
            FormatModel fm = new FormatModel();

            Assert.AreEqual(saf.SelectedText.SelectedText.SelectedString, _model.select.SelectedString);
            Assert.IsNull(saf.SelectedText.Format);
            Assert.AreEqual(saf.Align, Alignment.Left);
            Assert.AreEqual(saf.Formatting.Family, fm.Family);
            Assert.AreEqual(saf.Formatting.Size, fm.Size);
            Assert.AreEqual(saf.Formatting.Style, fm.Style);
            Assert.AreEqual(saf.Formatting.Weight, fm.Weight);
            Assert.AreEqual(saf.Formatting.Color, fm.Color);
            Assert.AreEqual(saf.Formatting.CharOffset, fm.CharOffset);
        }

        [TestMethod]
        public void DeleteAllFormattingTest()
        {
            FormatModel fm = new FormatModel();
            fm.SetDefaultFormatting();

            _model.SelectionAndFormat.DeleteAllFormatting();

            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Style, fm.Style);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Weight, fm.Weight);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Family, fm.Family);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Size, fm.Size);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Color, fm.Color);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.CharOffset, fm.CharOffset);
        }

        [TestMethod]
        public void SetBoldTest()
        {
            _model.SelectionAndFormat.SetBold();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Weight, "Bold");
        }

        [TestMethod]
        public void SetWeightTest()
        {
            _model.SelectionAndFormat.SetWeight("Heavy");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Weight, "Heavy");

            _model.SelectionAndFormat.SetWeight("Light");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Weight, "Light");

            _model.SelectionAndFormat.SetWeight("ultraBlack");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Weight, "ultraBlack");
        }

        [TestMethod]
        public void SetItalicTest()
        {
            _model.SelectionAndFormat.SetItalic();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Style, "Italic");
        }

        [TestMethod]
        public void SetObliqueTest()
        {
            _model.SelectionAndFormat.SetOblique();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Style, "Oblique");
        }

        [TestMethod]
        public void SetSubscriptTest()
        {
            _model.SelectionAndFormat.SetSuperscript();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.CharOffset, 2);
        }

        [TestMethod]
        public void SetSuperscriptTest()
        {
            _model.SelectionAndFormat.SetSubscript();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.CharOffset, -2);
        }

        [TestMethod]
        public void DeleteSubSuperscriptTest()
        {
            _model.SelectionAndFormat.DeleteSubSuperscript();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.CharOffset, 1);
        }

        [TestMethod]
        public void DeleteWeightTest()
        {
            _model.SelectionAndFormat.DeleteWeight();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Weight, "Normal");
        }

        [TestMethod]
        public void DeleteStyleTest()
        {
            _model.SelectionAndFormat.DeleteStyle();
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Style, "Normal");
        }

        [TestMethod]
        public void ChangeSizeTest()
        {
            _model.SelectionAndFormat.ChangeSize(14);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Size, 14);

            _model.SelectionAndFormat.ChangeSize(48);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Size, 48);
        }

        [TestMethod]
        public void ChangeFamilyTest()
        {
            _model.SelectionAndFormat.ChangeFont("Helvetica");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Family, "Helvetica");

            _model.SelectionAndFormat.ChangeFont("Courier new");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Family, "Courier new");

        }

        [TestMethod]
        public void ChangeColorTest()
        {
            _model.SelectionAndFormat.ChangeColor("050304");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Color, "#050304");

            _model.SelectionAndFormat.ChangeColor("d30e71");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Color, "#d30e71");

            _model.SelectionAndFormat.ChangeColor("FFFFFF");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Color, "#FFFFFF");
        }

        [TestMethod]
        public void SetLeftAlignmentTest()
        {
            _model.SelectionAndFormat.SetLeftAlignment();
            Assert.AreEqual(_model.SelectionAndFormat.Align, Alignment.Left);
        }

        [TestMethod]
        public void SetRightAlignmentTest()
        {
            _model.SelectionAndFormat.SetRightAlignment();
            Assert.AreEqual(_model.SelectionAndFormat.Align, Alignment.Right);
        }

        [TestMethod]
        public void SetCenterAlignmentTest()
        {
            _model.SelectionAndFormat.SetCenterAlignment();
            Assert.AreEqual(_model.SelectionAndFormat.Align, Alignment.Center);
        }

        [TestMethod]
        public void SetJustifyAlignmentTest()
        {
            _model.SelectionAndFormat.SetJustifyAlignment();
            Assert.AreEqual(_model.SelectionAndFormat.Align, Alignment.Justify);
        }

        [TestMethod]
        public void ChangeTextTest()
        {
            Selection temp = _model.SelectionAndFormat.GetSelection();

            _model.SelectionAndFormat.SetSelectedString("changed");

            Assert.AreEqual(_model.SelectionAndFormat.GetSelectedText(), "changed");
            Assert.AreEqual(_model.SelectionAndFormat.SelectedText.SelectedText.StartPointer, temp.StartPointer);
            Assert.AreEqual(_model.SelectionAndFormat.SelectedText.SelectedText.EndPointer, temp.StartPointer + 7);
        }
        #endregion

        #region Main model tests
        [TestMethod]
        public void ReverseWordTest()
        {
            Assert.AreEqual(_model.ReverseWord("abcd"), "dcba");
            Assert.AreEqual(_model.ReverseWord("test string"), "gnirts tset");
            Assert.AreEqual(_model.ReverseWord("Test Camel case String"), "gnirtS esac lemaC tseT");
            Assert.AreEqual(_model.ReverseWord("word1with2numbers3"), "3srebmun2htiw1drow");
        }

        //string family, int size, string style, string weight, string color
        [TestMethod]
        public void GetFormattingTest()
        {
            _model.SelectionAndFormat.SelectedText.Format = new FormatModel[5];

            _model.SelectionAndFormat.SelectedText.Format[0] = new FormatModel("Arial", 12, "Italic", "Bold", "#000000");
            _model.SelectionAndFormat.SelectedText.Format[1] = new FormatModel("Arial", 14, "Italic", "Bold", "#050304");
            _model.SelectionAndFormat.SelectedText.Format[2] = new FormatModel("Arial", 13, "Italic", "Normal", "#000000");
            _model.SelectionAndFormat.SelectedText.Format[3] = new FormatModel("Couries new", 16, "Italic", "Normal", "#050304");
            _model.SelectionAndFormat.SelectedText.Format[4] = new FormatModel("Arial", 12, "Italic", "Normal", "#000000");

            _model.GetFormatting();

            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Color, "#000000");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Family, "Arial");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Size, 12);
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Style, "Italic");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.Weight, "Normal");
            Assert.AreEqual(_model.SelectionAndFormat.Formatting.CharOffset, 1);
        }

        [TestMethod]
        public void AddNewFormatStyleTest()
        {
            Dictionary<string, FormatModel> before = new Dictionary<string, FormatModel>();
            foreach(string key in _model.FontStyles.Keys)
            {
                before.Add(key, _model.FontStyles[key]);
            }

            FormatModel fm = new FormatModel();
            fm.SetDefaultFormatting();

            _model.AddNewFormatStyle(fm);

            Assert.AreEqual(_model.FontStyles.Count, before.Count + 1);
            Assert.AreEqual(_model.FontStyles["Stílus0"].Family, fm.Family);
            Assert.AreEqual(_model.FontStyles["Stílus0"].Style, fm.Style);
            Assert.AreEqual(_model.FontStyles["Stílus0"].Weight, fm.Weight);
            Assert.AreEqual(_model.FontStyles["Stílus0"].Size, fm.Size);
            Assert.AreEqual(_model.FontStyles["Stílus0"].Color, fm.Color);
            Assert.AreEqual(_model.FontStyles["Stílus0"].CharOffset, fm.CharOffset);
        }

        [TestMethod]
        public void SetDefaultFormattingTest()
        {
            _model.SetDefaultFormatting();
            FormatModel fm = new FormatModel();
            fm.SetDefaultFormatting();
            MarginModel mm = new MarginModel();
            mm.SetDefaultMargin();

            Assert.AreEqual(_model.FormatText.Style, fm.Style);
            Assert.AreEqual(_model.FormatText.Weight, fm.Weight);
            Assert.AreEqual(_model.FormatText.Family, fm.Family);
            Assert.AreEqual(_model.FormatText.Size, fm.Size);
            Assert.AreEqual(_model.FormatText.Color, fm.Color);
            Assert.AreEqual(_model.FormatText.CharOffset, fm.CharOffset);

            Assert.AreEqual(_model.Align, Alignment.Center);

            Assert.AreEqual(_model.Margin.Right, mm.Right);
            Assert.AreEqual(_model.Margin.Left, mm.Left);
            Assert.AreEqual(_model.Margin.Bottom, mm.Bottom);
            Assert.AreEqual(_model.Margin.Top, mm.Top);
        }

        [TestMethod]
        public void LineHeightIncrTest1()
        {
            Assert.AreEqual(_model.LineHeight + 2, _model.LineHeightIncr());
            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(_model.LineHeight + 2, _model.LineHeightIncr());
            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(_model.LineHeight + 2, _model.LineHeightIncr());
            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(_model.LineHeight + 2, _model.LineHeightIncr());
        }

        [TestMethod]
        public void LineHeightIncrTest2()
        {
            _model.LineHeight = 48;
            Assert.AreEqual(_model.LineHeight + 2, _model.LineHeightIncr());
            _model.LineHeight = _model.LineHeightIncr();

            Assert.AreEqual(50, _model.LineHeight);

            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(50, _model.LineHeight);

            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(50, _model.LineHeight);
        }

        [TestMethod]
        public void LineHeightDecrTest1()
        {
            Assert.AreEqual(_model.LineHeight - 2, _model.LineHeightDecr());
            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(_model.LineHeight - 2, _model.LineHeightDecr());
            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(_model.LineHeight - 2, _model.LineHeightDecr());
            _model.LineHeight = _model.LineHeightIncr();
            Assert.AreEqual(_model.LineHeight - 2, _model.LineHeightDecr());
        }

        [TestMethod]
        public void LineHeightDecrTest2()
        {
            _model.LineHeight = 8;
            Assert.AreEqual(_model.LineHeight - 2, _model.LineHeightDecr());
            _model.LineHeight = _model.LineHeightDecr();

            Assert.AreEqual(6, _model.LineHeight);

            _model.LineHeight = _model.LineHeightDecr();
            Assert.AreEqual(6, _model.LineHeight);

            _model.LineHeight = _model.LineHeightDecr();
            Assert.AreEqual(6, _model.LineHeight);
        }

        [TestMethod]
        public void SetTopMarginTest()
        {
            _model.Margin.Top = _model.SetTopMargin(4);
            Assert.AreEqual(80, _model.Margin.Top);

            _model.Margin.Top = _model.SetTopMargin(-2);
            Assert.AreEqual(0, _model.Margin.Top);

            _model.Margin.Top = _model.SetTopMargin(-242517);
            Assert.AreEqual(0, _model.Margin.Top);

            _model.Margin.Top = _model.SetTopMargin(6);
            Assert.AreEqual(100, _model.Margin.Top);

            _model.Margin.Top = _model.SetTopMargin(17654);
            Assert.AreEqual(100, _model.Margin.Top);
        }

        [TestMethod]
        public void SetBottomMarginTest()
        {
            _model.Margin.Bottom = _model.SetBottomMargin(2);
            Assert.AreEqual(40, _model.Margin.Bottom);

            _model.Margin.Bottom = _model.SetBottomMargin(-4);
            Assert.AreEqual(0, _model.Margin.Bottom);

            _model.Margin.Bottom = _model.SetBottomMargin(-242517);
            Assert.AreEqual(0, _model.Margin.Bottom);

            _model.Margin.Bottom = _model.SetBottomMargin(8);
            Assert.AreEqual(100, _model.Margin.Bottom);

            _model.Margin.Bottom = _model.SetBottomMargin(735354);
            Assert.AreEqual(100, _model.Margin.Bottom);
        }

        [TestMethod]
        public void SetLeftMarginTest()
        {
            _model.Margin.Left = _model.SetLeftMargin(4);
            Assert.AreEqual(80, _model.Margin.Left);

            _model.Margin.Left = _model.SetLeftMargin(-2);
            Assert.AreEqual(0, _model.Margin.Left);

            _model.Margin.Left = _model.SetLeftMargin(-2517);
            Assert.AreEqual(0, _model.Margin.Left);

            _model.Margin.Left = _model.SetLeftMargin(6);
            Assert.AreEqual(100, _model.Margin.Left);

            _model.Margin.Left = _model.SetLeftMargin(1442654);
            Assert.AreEqual(100, _model.Margin.Left);
        }

        [TestMethod]
        public void SetRightMarginTest()
        {
            _model.Margin.Right = _model.SetRightMargin(3);
            Assert.AreEqual(60, _model.Margin.Right);

            _model.Margin.Right = _model.SetRightMargin(-6);
            Assert.AreEqual(0, _model.Margin.Right);

            _model.Margin.Right = _model.SetRightMargin(-1615017);
            Assert.AreEqual(0, _model.Margin.Right);

            _model.Margin.Right = _model.SetRightMargin(6);
            Assert.AreEqual(100, _model.Margin.Right);

            _model.Margin.Right = _model.SetRightMargin(11615654);
            Assert.AreEqual(100, _model.Margin.Right);
        }

        [TestMethod]
        public void SetAllMarginsTest1()
        {
            _model.SetAllMargins(3.5);
            Assert.AreEqual(70, _model.Margin.Right);
            Assert.AreEqual(70, _model.Margin.Left);
            Assert.AreEqual(70, _model.Margin.Top);
            Assert.AreEqual(70, _model.Margin.Bottom);

            _model.SetAllMargins(5);
            Assert.AreEqual(100, _model.Margin.Right);
            Assert.AreEqual(100, _model.Margin.Left);
            Assert.AreEqual(100, _model.Margin.Top);
            Assert.AreEqual(100, _model.Margin.Bottom);
        }


        [TestMethod]
        public void SetAllMarginsTest2()
        {
            _model.SetAllMargins(-3.5);
            Assert.AreEqual(0, _model.Margin.Right);
            Assert.AreEqual(0, _model.Margin.Left);
            Assert.AreEqual(0, _model.Margin.Top);
            Assert.AreEqual(0, _model.Margin.Bottom);

            _model.SetAllMargins(-354323214133423);
            Assert.AreEqual(0, _model.Margin.Right);
            Assert.AreEqual(0, _model.Margin.Left);
            Assert.AreEqual(0, _model.Margin.Top);
            Assert.AreEqual(0, _model.Margin.Bottom);
        }


        [TestMethod]
        public void SetAllMarginsTest3()
        {
            _model.SetAllMargins(7);
            Assert.AreEqual(100, _model.Margin.Right);
            Assert.AreEqual(100, _model.Margin.Left);
            Assert.AreEqual(100, _model.Margin.Top);
            Assert.AreEqual(100, _model.Margin.Bottom);

            _model.SetAllMargins(88765.3);
            Assert.AreEqual(100, _model.Margin.Right);
            Assert.AreEqual(100, _model.Margin.Left);
            Assert.AreEqual(100, _model.Margin.Top);
            Assert.AreEqual(100, _model.Margin.Bottom);
        }
        #endregion

        #region Selection tests
        [TestMethod]
        public void UpdateSelectionTest1()
        {
            Selection test = new Selection(6, 12, "select");
            test.AddToFront_Selected("add", 3);

            Assert.IsNotNull(test);
            Assert.AreEqual("addselect", test.SelectedString);
            Assert.AreEqual(3, test.StartPointer);
            Assert.AreEqual(12, test.EndPointer);
        }

        [TestMethod]
        public void UpdateSelectionTest2()
        {
            Selection test = new Selection(6, 12, "select");
            test.AddToEnd_Selected("add", 15);

            Assert.IsNotNull(test);
            Assert.AreEqual("selectadd", test.SelectedString);
            Assert.AreEqual(6, test.StartPointer);
            Assert.AreEqual(15, test.EndPointer);
        }

        [TestMethod]
        public void UpdateSelectionTest3()
        {
            Selection test = new Selection(6, 12, "select");
            test.AddToSelected("new", 3, 6);

            Assert.IsNotNull(test);
            Assert.AreEqual("new", test.SelectedString);
            Assert.AreEqual(3, test.StartPointer);
            Assert.AreEqual(6, test.EndPointer);
        }

        [TestMethod]
        public void UpdateSelectionTest4()
        {
            Selection test = new Selection(6, 12, "select");
            test.DeleteSelection();

            Assert.IsNotNull(test);
            Assert.IsNull(test.SelectedString);
            Assert.AreEqual(-1, test.StartPointer);
            Assert.AreEqual(-1, test.EndPointer);
        }
        #endregion
    }
}
