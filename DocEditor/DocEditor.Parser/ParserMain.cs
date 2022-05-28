using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using DocEditor.Model;

namespace DocEditor.Parser
{
    /// <summary>
    /// DocEditor parser class
    /// This class is for auto formatting the text's words which are already 
    /// stored in a data set called "dictionary" and managing the "dictionary".
    /// Dictionary: a list of DictClass elements
    /// Also implements a basic autocorrect algorithm by computing Levenshtein distance.
    /// </summary>
    public class ParserMain
    {
        #region Private fields and Public properties
        private DictClass _dict;
        private string[] _format_array;
        
        private string mostFreqStyle;
        private string mostFreqWeight;
        private string mostFreqFamily;
        private int mostFreqOffset;
        private int mostFreqSize;
        private string mostFreqColor;

        public List<DictClass> Dict;
        public string DictFileName;
        #endregion

        #region Constructors
        public ParserMain()
        {
            Dict = new List<DictClass>();
            _dict = new DictClass(null, null, 0);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Generates the dictionary format from the font style
        /// </summary>
        /// <param name="style">Font style</param>
        /// <returns></returns>
        private char StyleToChar(string style) 
        {
            switch (style)
            {
                case "Italic":
                    return 'i';
                case "Normal":
                    return 'n';
                case "Oblique":
                    return 'o';
                default:
                    return 'n';
            }
        }

        /// <summary>
        /// Generates the dictionary format from the font weight
        /// </summary>
        /// <param name="weight">Font weight</param>
        /// <returns>The character for the format</returns>
        private char WeightToChar(string weight)
        {
            switch (weight)
            {
                case "Bold":
                    return 'b';
                case "Normal":
                    return 'n';
                case "Light":
                    return 'l';
                default:
                    return 'n';
            }
        }

        /// <summary>
        /// Generate the dictionary format from the character offset
        /// 2: sperscript, -2: subscript, 1:normal
        /// </summary>
        /// <param name="offset">Character offset</param>
        /// <returns></returns>
        private char OffsetToChar(int offset)
        {
            switch (offset)
            {
                case 2:
                    return 'p';
                case -2:
                    return 'q';
                default:
                    return 'n';
            }
        }

        /// <summary>
        /// Generate from the dictionary form the actual formatting keyword
        /// </summary>
        /// <param name="offset">Character offset</param>
        /// <returns></returns>
        private int CharToOffset(char offset)
        {
            switch (offset)
            {
                case 'p':
                    return 2;
                case 'q':
                    return -2;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Generate from the dictionary form the actual font weight keyword
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        private string CharToWeight(char weight)    //TODO
        {
            switch (weight)
            {
                case 'b':
                    return "Bold";
                case 'n':
                    return "Normal";
                case 'l':
                    return "Light";
                default:
                    return "Normal";
            }
        }

        /// <summary>
        /// Generate from the dictionary form the actual font style keyword
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private string CharToStyle(char style)
        {
            switch (style)
            {
                case 'i':
                    return "Italic";
                case 'n':
                    return "Normal";
                case 'o':
                    return "Oblique";
                default:
                    return "Normal";
            }
        }

        /// <summary>
        /// Get the most frequent formatting to the first index of the format array
        /// </summary>
        /// <param name="ft_arr">The formatting for all characters of the string</param>
        private void GetTheOccurences(FormatModel[] ft_arr)
        {
            var styleFreq = new Dictionary<string, int>(ft_arr.Length);
            var weightFreq = new Dictionary<string, int>(ft_arr.Length);
            var familyFreq = new Dictionary<string, int>(ft_arr.Length);
            var chOffsetFreq = new Dictionary<int, int>(ft_arr.Length);
            var sizeFreq = new Dictionary<int, int>(ft_arr.Length);
            var colorFreq = new Dictionary<string, int>(ft_arr.Length);

            for (int i = 0; i < ft_arr.Length; i++)
            {
                if (styleFreq.ContainsKey(ft_arr[i].Style))
                {
                    styleFreq[ft_arr[i].Style]++;
                }
                else
                {
                    styleFreq.Add(ft_arr[i].Style, 1);
                }

                if (weightFreq.ContainsKey(ft_arr[i].Weight))
                {
                    weightFreq[ft_arr[i].Weight]++;
                }
                else
                {
                    weightFreq.Add(ft_arr[i].Weight, 1);
                }

                if (familyFreq.ContainsKey(ft_arr[i].Family))
                {
                    familyFreq[ft_arr[i].Family]++;
                }
                else
                {
                    familyFreq.Add(ft_arr[i].Family, 1);
                }

                if (chOffsetFreq.ContainsKey(ft_arr[i].CharOffset))
                {
                    chOffsetFreq[ft_arr[i].CharOffset]++;
                }
                else
                {
                    chOffsetFreq.Add(ft_arr[i].CharOffset, 1);
                }

                if (sizeFreq.ContainsKey(ft_arr[i].Size))
                {
                    sizeFreq[ft_arr[i].Size]++;
                }
                else
                {
                    sizeFreq.Add(ft_arr[i].Size, 1);
                }

                if (colorFreq.ContainsKey(ft_arr[i].Color))
                {
                    colorFreq[ft_arr[i].Color]++;
                }
                else
                {
                    colorFreq.Add(ft_arr[i].Color, 1);
                }
                
            }

            mostFreqStyle = styleFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += StyleToChar(mostFreqStyle);
            _format_array[0] += ',';

            mostFreqWeight = weightFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += WeightToChar(mostFreqWeight);
            _format_array[0] += ',';

            mostFreqFamily = familyFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += mostFreqFamily;
            _format_array[0] += ',';

            mostFreqOffset = chOffsetFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += OffsetToChar(mostFreqOffset);
            _format_array[0] += ',';

            mostFreqSize = sizeFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += mostFreqSize;
            _format_array[0] += ',';

            mostFreqColor = colorFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += mostFreqColor;

        }

        /// <summary>
        /// Generate the dictionary form formatting to one of the carachters
        /// </summary>
        /// <param name="ft"></param>
        /// <returns></returns>
        private string GenerateFormat(FormatModel ft)
        {
            string res = null;
            if(!Equals(ft.Style, mostFreqStyle))
            {
                res += StyleToChar(ft.Style);
                res += ',';
            }
            else
            {
                res += "_,";
            }

            if (!Equals(ft.Weight, mostFreqWeight))
            {
                res += WeightToChar(ft.Weight);
                res += ',';
            }
            else
            {
                res += "_,";
            }

            if (!Equals(ft.Family, mostFreqFamily))
            {
                res += ft.Family;
                res += ',';
            }
            else
            {
                res += "_,";
            }

            if (!Equals(ft.CharOffset, mostFreqOffset))
            {
                res += OffsetToChar(ft.CharOffset);
                res += ',';
            }
            else
            {
                res += "_,";
            }

            if (!Equals(ft.Size, mostFreqSize))
            {
                res += ft.Size;
                res += ',';
            }
            else
            {
                res += "_,";
            }

            if (!Equals(ft.Color, mostFreqColor))
            {
                res += ft.Color;
                res += ',';
            }
            else
            {
                res += "_,";
            }

            return res;
        }

        /// <summary>
        /// Helper function for adding a new element to the dictionary
        /// </summary>
        /// <param name="ft_arr">The formatting informations for all character</param>
        private void StwfFormatToDictFormat(FormatModel[] ft_arr)
        {
            //arr[0]: the most frequent formatting, if there isnt any, then '_'
            //arr[n]: Style Weight CharOffset Family Size Color
            //kiválasztani a legtöbbet elõforduló dolgokat 
            //FormatModel[n].Style    
            _format_array = new string[ft_arr.Length + 1];

            GetTheOccurences(ft_arr); //_format_array[0] done

            for (int i = 0; i<ft_arr.Length; i++)
            {
                _format_array[i+1] = GenerateFormat(ft_arr[i]);
            }
        }

        /// <summary>
        /// Adding a new element to the dictionary
        /// </summary>
        /// <param name="st">The element to add</param>
        private void FromStwfToDict(Stwf st)
        {
            StwfFormatToDictFormat(st.Format);
            //creating a new dictionary element for further checks
            _dict = new DictClass(st.SelectedText.SelectedString, _format_array, 1);
        }

        /// <summary>
        /// Generating formatting for a character by comparing the default and the actual formatting
        /// </summary>
        /// <param name="def">Default formatting</param>
        /// <param name="df">Actual formatting</param>
        /// <returns></returns>
        private FormatModel GenerateFormatModel(string def, string df)
        {
            //arr[n]: Style[0] Weight[1] CharOffset[2] Family[3] Size[4] Color[5]
            string[] formatArray = df.Split(",");

            string[] def_arr = def.Split(",");
            int i = 0;
            foreach(string d in formatArray)
            {
                if (String.Equals(d, "_"))
                {
                    formatArray[i] = def_arr[i];
                }
                i++;
            }
    
            //FormatModel(string family, int size, string style, string weight, string color){}
            FormatModel res = new FormatModel(CharToStyle(char.Parse(formatArray[0])), CharToWeight(char.Parse(formatArray[1])), CharToOffset(char.Parse(formatArray[3])), formatArray[2], int.Parse(formatArray[4]), formatArray[5]);
            return res;
        }

        /// <summary>
        /// Helper function for getting a word's formatting from the dictionary 
        /// </summary>
        /// <param name="df">The formatting</param>
        /// <param name="len">The length of the word</param>
        /// <returns></returns>
        private FormatModel[] DictFormatToStwfFormat(string[] df, int len)
        {
            //df[0] - default formatting, where there is a '_' should be the format from here
            //arr[n]: Style Weight CharOffset Family Size Color
            FormatModel[] fm = new FormatModel[len];
            
            for(int i=0; i<df.Length-1; i++)
            {
                fm[i] = GenerateFormatModel(df[0], df[i+1]);
            }

            return fm;
        }

        /// <summary>
        /// Getting a word's formatting from the dictionary
        /// </summary>
        /// <param name="dc">The dictionary element</param>
        /// <returns></returns>
        private FormatModel[] FromDictToStwf(DictClass dc)
        {
            return DictFormatToStwfFormat(dc.Formatting, dc.Str.Length);
        }

        /// <summary>
        /// Levenshtein distance computing for all words from the dictionary
        /// </summary>
        /// <param name="str">The word</param>
        /// <returns></returns>
        private int[] LevenshteinDistanceForAll(string str)
        {
            int[] res = new int[Dict.Count];
            int i = 0;
            foreach(var r in Dict)
            {
                res[i] = Levenshtein(str, r.Str);
                i++;
            }
            return res;
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Get the Levenshtein distance for two given strings
        /// </summary>
        /// <param name="str">string 1</param>
        /// <param name="dictStr">string 2</param>
        /// <returns></returns>
        public int Levenshtein(string str, string dictStr)
        {
            //edit distance for str and dictStr
            int n = str.Length;
            int m = dictStr.Length;
            int[,] levTable = new int[n + 1, m + 1];
            for (int i = 0; i < n + 1; i++)
            {
                levTable[i, 0] = i;
            }
            for (int i = 1; i < m + 1; i++)
            {
                levTable[0, i] = i;
            }

            for (int i = 1; i < n + 1; i++)
            {
                for (int j = 1; j < m + 1; j++)
                {
                    levTable[i, j] = str[i - 1] == dictStr[j - 1]
                        ? levTable[i - 1, j - 1]
                        : Math.Min(levTable[i - 1, j - 1], Math.Min(levTable[i - 1, j], levTable[i, j - 1])) + 1;
                }
            }
            return levTable[str.Length, dictStr.Length];
        }

        /// <summary>
        /// Trims the whitespaces from the end and the beginning of the selected string
        /// </summary>
        /// <param name="s">The selection from the document</param>
        /// <returns>The trimmed selection</returns>
        public Selection SelectionTrim(Selection s)
        {
            //setting the text pointers
            int st = s.SelectedString.Length - s.SelectedString.TrimStart(' ').Length;
            s.StartPointer += st;
            int nd = s.SelectedString.Length - s.SelectedString.TrimEnd(' ').Length;
            s.EndPointer -= nd;

            s.SelectedString = s.SelectedString.Trim(' ');

            return s;
        }

        /// <summary>
        /// The validation of the selected text after the trimming
        /// </summary>
        /// <param name="s">The selected text with its pointers</param>
        /// <returns></returns>
        public Selection SelectedTextValidation(Selection s, string strBefore, string strAfter)
        {
            //check if the whole word is selected
            if((strBefore == " " || strBefore == "" || strBefore.Any(c => char.IsSymbol(c)) || strBefore.Any(c => char.IsPunctuation(c)))
                && (strAfter == " " || strAfter == "" || strAfter.Any(c => char.IsSymbol(c)) || strAfter.Any(c => char.IsPunctuation(c))))
            {
                //check if it doesnt contains whitespaces (only one word)
                if (s.SelectedString.Any(c => char.IsWhiteSpace(c)) || s.SelectedString.Any(c => char.IsSymbol(c)) || s.SelectedString.Any(c => char.IsPunctuation(c)))
                {
                    s.SelectedString = null;
                }
            }
            else
            {
                s.SelectedString = null;
            }

            //if wrong, s.SelectedString = null
            return s;
        }

        /// <summary>
        /// Adding a new element to the dictionary
        /// </summary>
        /// <param name="str"></param>
        public void ToDictionary(Stwf str)
        {
            FromStwfToDict(str);
            //if the string, formatting and so already exists then update dictionary content
            //search + DictClass.Freq++
            int ind = Dict.FindIndex(x => String.Equals(x.Str, _dict.Str) && Enumerable.SequenceEqual(x.Formatting, _dict.Formatting));
            if ( ind != -1)
            {
                Dict[ind].Frequency++;
            }
            else
            {
                //else add new element
                Dict.Add(_dict);
            }
        }

        /// <summary>
        /// Gets the most possible formatting from the dictionary
        /// </summary>
        /// <param name="str"></param>
        /// <returns>The formatting applied to the text</returns>
        public Stwf FromDictionary(Stwf str)
        {
            if (ContainsElement(str.SelectedText.SelectedString))
            {
                //same string with the maximum frequency
                int maxFreq = Dict.Where(x => x.Str == str.SelectedText.SelectedString).Max(x => x.Frequency);
                int ind = Dict.FindIndex(x => x.Str == str.SelectedText.SelectedString && x.Frequency == maxFreq);

                //Dict[ind] is the element form the dictionary which we need to add to the text
                //Dict[ind].formatting -> stwf formatting
                str.Format = FromDictToStwf(Dict[ind]);

                Dict[ind].Frequency++;
            }

            //the text and the textpointers stay the same
            return str;
        }

        /// <summary>
        /// Get the first max 3 element with maximum edit distance 2
        /// </summary>
        /// <param name="str">The word with formatting</param>
        /// <returns></returns>
        public List<DictClass> GetEditDistance(Stwf str)
        {
            List<DictClass> res = new List<DictClass>();
            Dictionary<DictClass, int> levDict = new Dictionary<DictClass, int>();
            int[] tempArr = LevenshteinDistanceForAll(str.SelectedText.SelectedString);

            int ind = 0;
            foreach(var d in Dict)
            {
                if (ind < tempArr.Length)
                {
                    levDict.Add(d, tempArr[ind]);
                    ind++;
                }
            }

            IOrderedEnumerable<KeyValuePair<DictClass, int>> ordered = levDict.OrderBy(x => x.Value);

            ind = 0;
            foreach(var o in ordered)
            {
                if (o.Value <= 2)
                {
                    res.Add(o.Key);
                    ind++;
                    if (ind == 3)
                    {
                        return res;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Check if the dictionary already contains the string
        /// </summary>
        /// <param name="str">The word</param>
        /// <returns></returns>
        public bool ContainsElement(string str)
        {
            int ind = Dict.FindIndex(x => String.Equals(x.Str.ToLower(), str.ToLower()));
            if(ind != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remove elements from the dictionary by index
        /// </summary>
        /// <param name="ind">The index</param>
        public void RemoveElementFromDictionary(int ind)
        {
            if (Dict.Count > ind)
            {
                Dict.RemoveAt(ind);
            }
        }

        /// <summary>
        /// Remove the given element from the dictionary
        /// </summary>
        /// <param name="dc"></param>
        public void RemoveElementFromDictionary(DictClass dc)
        {
            if (Dict.Contains(dc))
            {
                Dict.Remove(dc);
            }
        }

        /// <summary>
        /// Get the formatting from the dictionary by an element
        /// </summary>
        /// <param name="dc">The dictionary element for the formatting</param>
        /// <returns></returns>
        public FormatModel[] GetFormatting(DictClass dc)
        {
            return FromDictToStwf(dc);
        }

        /// <summary>
        /// Sets the corrected text and position offsets
        /// </summary>
        public Selection GetPossibleFormatting(DictClass dc, Stwf st)
        {
            st.SelectedText.SelectedString = dc.Str;
            st.SelectedText.EndPointer = st.SelectedText.StartPointer + dc.Str.Length;
            return new Selection(st.SelectedText.StartPointer, st.SelectedText.EndPointer, st.SelectedText.SelectedString);
        }

        /// <summary>
        /// Gets all the possible dictionary elements by error correcting for strings longer than 3 charachters
        /// </summary>
        /// <param name="str"></param>
        /// <returns>The list of all the possible strings and formatting</returns>
        public List<Stwf> GetFromDictByLevenshtein(Stwf str)
        {
            List<Stwf> res = new List<Stwf>();
            int[] distances = LevenshteinDistanceForAll(str.SelectedText.SelectedString);
            int i = 0;
            foreach (var r in Dict)
            {
                //if the Levenshtein distance is less than 3, the item shoud be added to the result list
                if(distances[i] < 2)
                {
                    str.Format = FromDictToStwf(Dict[i]);
                    res.Add(str);
                }
                i++;
            }
            return res;
        }

        #endregion

        #region Json methods

        /// <summary>
        /// Method to write the dictionary list to a json file
        /// </summary>
        /// <param name="fileName">The name of the json file</param>
        public void DictToJson(string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonStr = JsonSerializer.Serialize(Dict, options);
            System.Diagnostics.Debug.WriteLine(jsonStr);

            File.WriteAllText(fileName, jsonStr);
            System.Diagnostics.Debug.WriteLine(File.ReadAllText(fileName));

        }

        /// <summary>
        /// Method to reading from the json file to the dictionary list
        /// </summary>
        /// <param name="fileName">The name of the json file</param>
        public void JsonToDict(string fileName)
        {
            string jsonStr = File.ReadAllText(fileName);
            Dict = JsonSerializer.Deserialize<List<DictClass>>(jsonStr)!;

            DictFileName = fileName;
            
        }

        /// <summary>
        /// Method to write the dictionary list to a json file
        /// </summary>
        public void DictToJson()
        {
            try
            {
                DictToJson(DictFileName);
            }
            catch (ArgumentNullException) { }              
        }

        /// <summary>
        /// Method to reading from the json file to the dictionary list
        /// </summary>
        public void JsonToDict()
        {
            try
            {
                JsonToDict(DictFileName);
            }
            catch (ArgumentException) { }          
        }
        #endregion
    }
}
