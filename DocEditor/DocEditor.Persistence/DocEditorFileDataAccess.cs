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
                using (StreamReader reader = new StreamReader(path))
                {
                    /*//rtb path
                    string line = await reader.ReadLineAsync();
                    string[] values = new string[10];
                    values[0] = line;

                    //json path
                    line = await reader.ReadLineAsync();
                    values[1] = line;

                    //empty line
                    line = await reader.ReadLineAsync();

                    //page width
                    line = await reader.ReadLineAsync();
                    values[2] = line;

                    //page height
                    line = await reader.ReadLineAsync();
                    values[3] = line;

                    //page margins (left, right, top, bottom)
                    line = await reader.ReadLineAsync();
                    string[] margins = line.Split(' ');
                    values[4] = margins[0];
                    values[5] = margins[1];
                    values[6] = margins[2];
                    values[7] = margins[3];

                    //empty line
                    line = await reader.ReadLineAsync();

                    //line height
                    line = await reader.ReadLineAsync();
                    values[8] = line;

                    //format styles
                    line = await reader.ReadLineAsync();
                    values[9] = line;*/

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
                        /*//rtb path
                        writer.WriteLine(values[0]);
                        //json path
                        writer.WriteLine(values[1]);

                        await writer.WriteLineAsync();

                        //page width
                        writer.WriteLine(values[2]);

                        //page height
                        writer.WriteLine(values[3]);

                        //margins
                        writer.WriteLine(values[4] + " " + values[5] + " " + values[6] + " " + values[7]);

                        //line height
                        writer.WriteLine(values[8]);

                        await writer.WriteLineAsync();

                        //format styles
                        writer.WriteLine(values[9]);*/

                    }
            }
            catch
            {
                throw new DocEditorDataException("Error occurred during writing.");
            }
        }

    }
}
