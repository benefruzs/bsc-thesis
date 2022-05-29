using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Persistence
{
    public interface IDocEditorDataAccess
    {
        /// <summary>
        /// File loading method
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns></returns>
        Task<string[]> LoadAsync(string path);

        /// <summary>
        /// File saving method
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="values">The stored information</param>
        /// <returns></returns>
        Task SaveAsync(string path, String[] values);
    }
}
