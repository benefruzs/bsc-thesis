using System;

namespace DocEditor.Persistence
{
    /// <summary>
    /// DocEditor data exception type
    /// </summary>
    public class DocEditorDataException : Exception
    {
        /// <summary>
        /// Instatination of DocEditor data exception 
        /// </summary>
        public DocEditorDataException(String message) : base(message) { }
    }
}
