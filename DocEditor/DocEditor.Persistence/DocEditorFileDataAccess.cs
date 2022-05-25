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
        /// Loading binary file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>The loaded information</returns>
        public async Task<string[]> LoadAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            try
            {
                byte[] fileData = await Task.Run(() => File.ReadAllBytes(path));

                return fileData.Select(fileByte => fileByte.ToString()).ToArray();
            }
            catch
            {
                throw new DocEditorDataException("Error occured during reading.");
            }
        }

        public async Task SaveAsync(string path, string[] values)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (values == null)
                throw new ArgumentNullException("values");

            try
            {
                // convert the values into an array
                byte[] fileData = values.Select(value => Convert.ToByte(value)).ToArray();

                // kiírjuk a tartalmat a megadott fájlba
                await Task.Run(() => File.WriteAllBytes(path, fileData));
            }
            catch
            {
                throw new DocEditorDataException("Error occurred during writing.");
            }
        }

    }
}
