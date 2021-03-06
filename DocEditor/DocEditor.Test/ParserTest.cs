using System.Collections.Generic;
using DocEditor.Model;
using DocEditor.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DocEditor.Test
{
    [TestClass]
    public class ParserTest
    {
        #region Private fields
        private ParserMain _parser;
        #endregion

        #region Initialization
        [TestInitialize]
        public void Initialize()
        {
            _parser = new ParserMain();
        }

        #endregion

        #region Selection trim tests
        [TestMethod]
        public void SelectionTrimTest1()
        {
            //there isnt any whitespace to trim
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest2()
        {
            //there is one whitespace to trim at the beginning
            Selection testSel = new Selection();
            testSel.SelectedString = " test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 7;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(3, testSel.StartPointer);
            Assert.AreEqual(7, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest3()
        {
            //there is one whitespace to trim at the end
            Selection testSel = new Selection();
            testSel.SelectedString = "test ";
            testSel.StartPointer = 2;
            testSel.EndPointer = 7;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest4()
        {
            //there is one whitespace to trim at the beginning and at the end
            Selection testSel = new Selection();
            testSel.SelectedString = " test ";
            testSel.StartPointer = 2;
            testSel.EndPointer = 8;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(3, testSel.StartPointer);
            Assert.AreEqual(7, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest5()
        {
            //there are multiple whitespaces to trim at the beginning and at the end
            Selection testSel = new Selection();
            testSel.SelectedString = "   test   ";
            testSel.StartPointer = 2;
            testSel.EndPointer = 12;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(5, testSel.StartPointer);
            Assert.AreEqual(9, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest6()
        {
            //there are multiple whitespaces to trim at the beginning
            Selection testSel = new Selection();
            testSel.SelectedString = "    test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 10;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(6, testSel.StartPointer);
            Assert.AreEqual(10, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest7()
        {
            //there are multiple whitespaces to trim at the end
            Selection testSel = new Selection();
            testSel.SelectedString = "test    ";
            testSel.StartPointer = 2;
            testSel.EndPointer = 10;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest8()
        {
            //there are whitespaces in the text but not to trim
            Selection testSel = new Selection();
            testSel.SelectedString = "test test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 11;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(11, testSel.EndPointer);
            Assert.AreEqual("test test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest9()
        {
            //there are whitespaces in the text but not to trim
            Selection testSel = new Selection();
            testSel.SelectedString = "test  test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 12;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(12, testSel.EndPointer);
            Assert.AreEqual("test  test", testSel.SelectedString);
        }

        [TestMethod]
        public void SelectionTrimTest10()
        {
            //there are whitespaces in the text but not to trim
            Selection testSel = new Selection();
            testSel.SelectedString = " test test ";
            testSel.StartPointer = 1;
            testSel.EndPointer = 12;
            testSel = _parser.SelectionTrim(testSel);

            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(11, testSel.EndPointer);
            Assert.AreEqual("test test", testSel.SelectedString);
        }

        #endregion

        #region Text validation tests
        [TestMethod]
        public void ValidationTest1()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 11;

            testSel = _parser.SelectedTextValidation(testSel, " ", " ");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(11, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest2()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 11;

            testSel = _parser.SelectedTextValidation(testSel, "a", " ");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(11, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest3()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 11;

            testSel = _parser.SelectedTextValidation(testSel, " ", "a");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(11, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest4()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 11;

            testSel = _parser.SelectedTextValidation(testSel, "a", "a");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(11, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest5()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, "a", " ");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest6()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, " ", "a");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }


        [TestMethod]
        public void ValidationTest7()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, "a", "a");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest8()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, " ", " ");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest9()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, "", "");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest10()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, ".", ",");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest11()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, " ", "-");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest12()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            testSel = _parser.SelectedTextValidation(testSel, ":", " ");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(6, testSel.EndPointer);
            Assert.AreEqual("test", testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest13()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test-te";
            testSel.StartPointer = 2;
            testSel.EndPointer = 9;

            testSel = _parser.SelectedTextValidation(testSel, ",", " ");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(9, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        [TestMethod]
        public void ValidationTest14()
        {
            Selection testSel = new Selection();
            testSel.SelectedString = "test, te";
            testSel.StartPointer = 2;
            testSel.EndPointer = 10;

            testSel = _parser.SelectedTextValidation(testSel, " ", " ");
            Assert.AreEqual(2, testSel.StartPointer);
            Assert.AreEqual(10, testSel.EndPointer);
            Assert.AreEqual(null, testSel.SelectedString);
        }

        #endregion

        #region New element to the dictionary tests
        [TestMethod]
        public void toDictTest1()
        {
            _parser.Dict = GenerateDictWithOneElement();
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            FormatModel fm0 = new FormatModel();
            FormatModel fm1 = new FormatModel();
            FormatModel fm2 = new FormatModel();
            FormatModel fm3 = new FormatModel();
            FormatModel[] fm = new FormatModel[4] {fm0, fm1, fm2, fm3 };
            Stwf addToDict = new Stwf(testSel, fm);

            _parser.ToDictionary(addToDict);

            Assert.AreEqual(2, _parser.Dict.Count);
            Assert.AreEqual(_parser.Dict[1].Str, _parser.Dict[0].Str);
            Assert.AreEqual(1, _parser.Dict[1].Frequency);
        }

        [TestMethod]
        public void toDictTest2()
        {
            _parser.Dict = GenerateDictWithMoreElements();
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            FormatModel fm0 = new FormatModel();
            FormatModel fm1 = new FormatModel();
            FormatModel fm2 = new FormatModel();
            FormatModel fm3 = new FormatModel();
            FormatModel[] fm = new FormatModel[4] { fm0, fm1, fm2, fm3 };
            Stwf addToDict = new Stwf(testSel, fm);

            _parser.ToDictionary(addToDict);

            Assert.AreEqual(4, _parser.Dict.Count);
            Assert.AreEqual("test", _parser.Dict[3].Str);
            Assert.AreEqual(1, _parser.Dict[3].Frequency);
        }

        [TestMethod]
        public void toDictTest3()
        {
            _parser.Dict = GenerateDictWithOneElement();
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            //FormatModel(string family, int size, string style, string weight, string color)
            FormatModel fm0 = new FormatModel("Arial", 12, "Normal", "Bold", "Black");
            FormatModel fm1 = new FormatModel("Arial", 12, "Italic", "Normal", "Black");
            FormatModel fm2 = new FormatModel("Arial", 12, "Normal", "Bold", "Black");
            FormatModel fm3 = new FormatModel("Arial", 12, "Italic", "Bold", "Black");
            FormatModel[] fm = new FormatModel[4] { fm0, fm1, fm2, fm3 };
            Stwf addToDict = new Stwf(testSel, fm);

            _parser.ToDictionary(addToDict);

            Assert.AreEqual("i,b,Arial,n,12,Black", _parser.Dict[0].Formatting[0]);
            Assert.AreEqual("n,_,_,_,_,_,", _parser.Dict[0].Formatting[1]);
            Assert.AreEqual("_,n,_,_,_,_,", _parser.Dict[0].Formatting[2]);
            Assert.AreEqual("n,_,_,_,_,_,", _parser.Dict[0].Formatting[3]);
            Assert.AreEqual("_,_,_,_,_,_,", _parser.Dict[0].Formatting[4]);
            Assert.AreEqual(1, _parser.Dict.Count);
            Assert.AreEqual(4, _parser.Dict[0].Frequency);
        }

        [TestMethod]
        public void toDictTest4()
        {
            _parser.Dict = GenerateDictWithMoreElements();
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            //FormatModel(string family, int size, string style, string weight, string color)
            FormatModel fm0 = new FormatModel("Arial", 12, "Normal", "Bold", "Black");
            FormatModel fm1 = new FormatModel("Arial", 12, "Italic", "Normal", "Black");
            FormatModel fm2 = new FormatModel("Arial", 12, "Normal", "Bold", "Black");
            FormatModel fm3 = new FormatModel("Arial", 12, "Italic", "Bold", "Black");
            FormatModel[] fm = new FormatModel[4] { fm0, fm1, fm2, fm3 };
            Stwf addToDict = new Stwf(testSel, fm);

            _parser.ToDictionary(addToDict);

            Assert.AreEqual("i,b,Arial,n,12,Black", _parser.Dict[0].Formatting[0]);
            Assert.AreEqual("n,_,_,_,_,_,", _parser.Dict[0].Formatting[1]);
            Assert.AreEqual("_,n,_,_,_,_,", _parser.Dict[0].Formatting[2]);
            Assert.AreEqual("n,_,_,_,_,_,", _parser.Dict[0].Formatting[3]);
            Assert.AreEqual("_,_,_,_,_,_,", _parser.Dict[0].Formatting[4]);
            Assert.AreEqual(3, _parser.Dict.Count);
            Assert.AreEqual(4, _parser.Dict[0].Frequency);
        }
        #endregion

        #region Getting elements from the dictionary tests
        [TestMethod]
        public void fromDictTest1()
        {
            _parser.Dict = GenerateDictWithOneElement();
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            FormatModel fm0 = new FormatModel();
            FormatModel fm1 = new FormatModel();
            FormatModel fm2 = new FormatModel();
            FormatModel fm3 = new FormatModel();
            FormatModel[] fm = new FormatModel[4] { fm0, fm1, fm2, fm3 };
            Stwf addToDict = new Stwf(testSel, fm);

            _parser.ToDictionary(addToDict);

            Stwf getFromDict = new Stwf(testSel, null);

            Stwf res = _parser.FromDictionary(getFromDict);
            Assert.AreEqual(4, _parser.Dict[0].Frequency);
            Assert.AreEqual(1, _parser.Dict[1].Frequency);
            Assert.AreEqual("test", res.SelectedText.SelectedString);
            Assert.AreEqual(2, res.SelectedText.StartPointer);
            Assert.AreEqual(6, res.SelectedText.EndPointer);
            Assert.IsNotNull(res.Format);
        }
        #endregion

        #region Remove elements from the dictionary
        [TestMethod]
        public void RemoveTest1()
        {
            _parser.Dict = GenerateDictWithMoreElements();
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            FormatModel fm0 = new FormatModel();
            FormatModel fm1 = new FormatModel();
            FormatModel fm2 = new FormatModel();
            FormatModel fm3 = new FormatModel();
            FormatModel[] fm = new FormatModel[4] { fm0, fm1, fm2, fm3 };
            Stwf addToDict = new Stwf(testSel, fm);

            _parser.ToDictionary(addToDict);

            Assert.AreEqual(4, _parser.Dict.Count);

            _parser.RemoveElementFromDictionary(_parser.Dict[0]);

            Assert.AreEqual(3, _parser.Dict.Count);
        }

        [TestMethod]
        public void RemoveTest2()
        {
            _parser.Dict = GenerateDictWithMoreElements();
            Selection testSel = new Selection();
            testSel.SelectedString = "test";
            testSel.StartPointer = 2;
            testSel.EndPointer = 6;

            FormatModel fm0 = new FormatModel();
            FormatModel fm1 = new FormatModel();
            FormatModel fm2 = new FormatModel();
            FormatModel fm3 = new FormatModel();
            FormatModel[] fm = new FormatModel[4] { fm0, fm1, fm2, fm3 };
            Stwf addToDict = new Stwf(testSel, fm);

            _parser.ToDictionary(addToDict);

            Assert.AreEqual(4, _parser.Dict.Count);

            _parser.RemoveElementFromDictionary(1);

            Assert.AreEqual(3, _parser.Dict.Count);
        }
        #endregion

        #region Levenshtein tests
        [TestMethod]
        public void LevenshteinDistTest()
        {
            Assert.AreEqual(_parser.Levenshtein("str", "stt"), 1);
            Assert.AreEqual(_parser.Levenshtein("abc", "aaa"), 2);
            Assert.AreEqual(_parser.Levenshtein("test", "test"), 0);
            Assert.AreEqual(_parser.Levenshtein("sitting", "kitten"), 3);
        }

        [TestMethod]
        public void ContainsElementTrueTest()
        {
            _parser.Dict = GenerateDictWithMoreElements();

            Assert.IsTrue(_parser.ContainsElement("test"));
            Assert.IsTrue(_parser.ContainsElement("Test"));
            Assert.IsTrue(_parser.ContainsElement("TEST"));
            Assert.IsTrue(_parser.ContainsElement("stet"));
            Assert.IsTrue(_parser.ContainsElement("tset"));
        }

        [TestMethod]
        public void ContainsElementFalseTest()
        {
            _parser.Dict = GenerateDictWithMoreElements();

            Assert.IsFalse(_parser.ContainsElement("tttt"));
            Assert.IsFalse(_parser.ContainsElement("Ttttt"));
            Assert.IsFalse(_parser.ContainsElement("abcdef"));
            Assert.IsFalse(_parser.ContainsElement("t"));
        }
        #endregion

        #region Private methods
        private static List<DictClass> GenerateDictWithOneElement()
        {
            DictClass dictElement = new DictClass(null, null, 0);
            List<DictClass> dictList = new List<DictClass>();

            dictElement.Str = "test";
            //arr[n]: Style Weight CharOffset Family Size Color
            dictElement.Formatting = new string[5] { "i,b,Arial,n,12,Black", "n,_,_,_,_,_,", "_,n,_,_,_,_,", "n,_,_,_,_,_,", "_,_,_,_,_,_," };
            dictElement.Frequency = 3;
            dictList.Add(dictElement);

            return dictList;
        }

        private static List<DictClass> GenerateDictWithMoreElements()
        {
            DictClass dictElement = new DictClass(null, null, 0);
            List<DictClass> dictList = new List<DictClass>();

            dictElement.Str = "test";
            //arr[n]: Style Weight CharOffset Family Size Color
            dictElement.Formatting = new string[5] { "i,b,Arial,n,12,Black", "n,_,_,_,_,_,", "_,n,_,_,_,_,", "n,_,_,_,_,_,", "_,_,_,_,_,_," };
            dictElement.Frequency = 3;
            dictList.Add(dictElement);

            DictClass dictElement1 = new DictClass(null, null, 0);
            dictElement1.Str = "stet";
            dictElement1.Formatting = new string[5] { "n,n,Courier new,n,12,Red", "_,_,_,_,_,_,", "i,b,_,_,_,_,", "_,_,_,_,8,_,", "i,_,_,_,_,_," };
            dictElement1.Frequency = 6;
            dictList.Add(dictElement1);

            DictClass dictElement2 = new DictClass(null, null, 0);
            dictElement2.Str = "tset";
            dictElement2.Formatting = new string[5] { "n,n,Arial,n,12,Black", "_,_,_,_,_,Red", "i,_,_,_,_,_,", "_,_,_,_,_,_,", "_,b,_,_,_,_," };
            dictElement2.Frequency = 4;
            dictList.Add(dictElement2);

            return dictList;
        }
        #endregion
    }
}
