using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEditor.Persistence
{
    public class DocEditorFileDataAccess : IDocEditorDataAccess
    {
        /// <summary>
        /// Loading text file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>The loaded information</returns>
        public async Task<string[]> LoadAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string[] values = new string[10];
                    string line = await reader.ReadLineAsync();
                    for (int i = 0; i < 10; i++)
                    {                
                        values[i] = line;
                        if (i < 9)
                        {
                            line = await reader.ReadLineAsync();
                        }
                    }

                    return values;
                }
            }
            catch
            {
                throw new DocEditorDataException("Error occured during reading.");
            }
        }

        /// <summary>
        /// Saving text file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task SaveAsync(string path, string[] values)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (values == null)
                throw new ArgumentNullException("values");

            try
            {
                using (StreamWriter writer = new StreamWriter(path)) // fájl megnyitása
                {
                    for (int i = 0; i < 10; i++)
                    {
                        await writer.WriteLineAsync(values[i]);
                    }
                }
            }
            catch
            {
                throw new DocEditorDataException("Error occurred during writing.");
            }
        }

    }
}
