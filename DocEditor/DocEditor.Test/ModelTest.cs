using System.Collections.Generic;
using DocEditor.Model;
using DocEditor.Parser;
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
            _model = new DocEditorModel();
        }

        #endregion
    }
}
