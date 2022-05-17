using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using DocEditor.Model;

namespace DocEditor.Parser
{
    public class ParserMain
    {
        //kell egy adatszerkezet, amiben benne van minden a fájlból

        /*-	beírt formázott szó a formázás rárakása után odakerül az elemzõhöz, ami eldönti, hogy el kell-e tárolni 
         * (nem kell eltárolni, ha valószínûsíthetõen egyszeri elõfordulásról van szó)
         *-	egy szó után szóköz lenyomása után megnéz, hogy szerepel-e az eltárolt szavak között, ha igen, rárakja a formázást
        */

        //formázás után eldönteni, hogy el kell-e tárolni

        //formázatlan szóról eldönteni, hogy szerepel-e a dictionaryben

        //dictionary: szavak, formázásuk, színük

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
        /// Generate the dictionary format from the font style
        /// </summary>
        /// <param name="style">Font style</param>
        /// <returns></returns>
        private char styleToChar(string style) 
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
        /// Generate the dictionary format from the font weight
        /// </summary>
        /// <param name="weight">Font weight</param>
        /// <returns></returns>
        private char weightToChar(string weight)    //TODO
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
        private char offsetToChar(int offset)
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
        private int charToOffset(char offset)
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
        private string charToWeight(char weight)    //TODO
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
        private string charToStyle(char style)
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
        private void getTheOccurences(FormatModel[] ft_arr)
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
            _format_array[0] += styleToChar(mostFreqStyle);
            _format_array[0] += ',';

            mostFreqWeight = weightFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += weightToChar(mostFreqWeight);
            _format_array[0] += ',';

            mostFreqFamily = familyFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += mostFreqFamily;
            _format_array[0] += ',';

            mostFreqOffset = chOffsetFreq.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            _format_array[0] += offsetToChar(mostFreqOffset);
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
        private string generateFormat(FormatModel ft)
        {
            string res = null;
            if(!Equals(ft.Style, mostFreqStyle))
            {
                res += styleToChar(ft.Style);
                res += ',';
            }
            else
            {
                res += "_,";
            }

            if (!Equals(ft.Weight, mostFreqWeight))
            {
                res += weightToChar(ft.Weight);
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
                res += offsetToChar(ft.CharOffset);
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


        private void StwfFormatToDictFormat(FormatModel[] ft_arr)
        {
            //arr[0]: the most frequent formatting, if there isnt any, then '_'
            //arr[n]: Style Weight CharOffset Family Size Color
            //kiválasztani a legtöbbet elõforduló dolgokat 
            //FormatModel[n].Style    
            _format_array = new string[ft_arr.Length + 1];

            getTheOccurences(ft_arr); //_format_array[0] done

            for (int i = 0; i<ft_arr.Length; i++)
            {
                _format_array[i+1] = generateFormat(ft_arr[i]);
            }
        }

        private void fromStwfToDict(Stwf st)
        {
            StwfFormatToDictFormat(st.Format);
            //creating a new dictionary element for further checks
            _dict = new DictClass(st.SelectedText.SelectedString, _format_array, 1);
        }

        private FormatModel generateFormatModel(string def, string df)
        {
            //a vesszõk mentén fel kell bontani
            //arr[n]: Style[0] Weight[1] CharOffset[2] Family[3] Size[4] Color[5]
            string[] formatArray = df.Split(",");
            //formatarrayba vagy df-bõl, vagy ha ott '_', akkor def-bõl
            //a vesszõkig beolvas a str df és defbõl
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
            FormatModel res = new FormatModel(charToStyle(char.Parse(formatArray[0])), charToWeight(char.Parse(formatArray[1])), charToOffset(char.Parse(formatArray[3])), formatArray[2], int.Parse(formatArray[4]), formatArray[5]);
            return res;
        }


        private FormatModel[] DictFormatToStwfFormat(string[] df, int len)
        {
            //df[0] - default formatting, where there is a '_' should be the format from here
            //arr[n]: Style Weight CharOffset Family Size Color
            FormatModel[] fm = new FormatModel[len];
            
            for(int i=0; i<df.Length-1; i++)
            {
                fm[i] = generateFormatModel(df[0], df[i+1]);
            }

            return fm;
        }

        private FormatModel[] fromDictToStwf(DictClass dc)
        {
            return DictFormatToStwfFormat(dc.Formatting, dc.Str.Length);
        }

        private int Levenshtein(string str, string dictStr)
        {

            return 0;
        }

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
        public Selection selectionTrim(Selection s)
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
        public Selection selectedTextValidation(Selection s, string strBefore, string strAfter)
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
        public void toDictionary(Stwf str)
        {
            fromStwfToDict(str);
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
        public Stwf fromDictionary(Stwf str)
        {
            //same string with the maximum frequency
            int maxFreq = Dict.Where(x => x.Str == str.SelectedText.SelectedString).Max(x => x.Frequency);
            int ind = Dict.FindIndex(x => x.Str == str.SelectedText.SelectedString && x.Frequency == maxFreq);

            //Dict[ind] is the element form the dictionary which we need to add to the text
            //Dict[ind].formatting -> stwf formatting
            str.Format = fromDictToStwf(Dict[ind]);

            Dict[ind].Frequency++;

            //the text and the textpointers stay the same
            return str;
        }

        /// <summary>
        /// Gets all the possible dictionary elements which has different formattings to the same text
        /// </summary>
        /// <param name="str"></param>
        /// <returns>all possible formatting to the current text</returns>
        public List<Stwf> getAllPossibleFormatting(Stwf str)
        {
            List<Stwf> res = new List<Stwf>();
            List<DictClass> fromDict = Dict.FindAll(x => x.Str == str.SelectedText.SelectedString);
            int i = 0;
            foreach(var r in fromDict)
            {
                res[i].Format = fromDictToStwf(r);
                i++;
            }
            return res;
        }

        /// <summary>
        /// Gets all the possible dictionary elements by error correcting for strings longer than 3 charachters
        /// </summary>
        /// <param name="str"></param>
        /// <returns>The list of all the possible strings and formatting</returns>
        public List<Stwf> getFromDictByLevenshtein(Stwf str)
        {
            List<Stwf> res = new List<Stwf>();
            int[] distances = LevenshteinDistanceForAll(str.SelectedText.SelectedString);
            int i = 0;
            foreach (var r in Dict)
            {
                //if the Levenshtein distance is less than 3, the item shoud be added to the result list
                if(distances[i] > 3)
                {
                    str.Format = fromDictToStwf(Dict[i]);
                    res.Add(str);
                }
                i++;
            }
            return res;
        }

        /// <summary>
        /// Method to write the dictionary list to a json file
        /// </summary>
        /// <param name="fileName">The name of the json file</param>
        public void dictToJson(string fileName)
        {
            //_dict = new DictClass(null, null, 0);
            //Dict = new List<DictClass>();

            //_dict.Str = "test";
            ////arr[n]: Style Weight CharOffset Family Size Color
            //_dict.Formatting = new string[5]{ "i,b,1,Arial,12,Black", "_,_,_,_,_,_", "n,n,_,_,_,_", "_,_,_,_,_,_", "n,_,_,_,_,_"};
            //_dict.Frequency = 3;
            //Dict.Add(_dict);

            //_dict.Str = "stet";
            ////arr[n]: Style Weight CharOffset Family Size Color
            //_dict.Formatting = new string[5] { "i,b,1,Arial,12,Black", "_,_,_,_,_,_", "n,n,_,_,_,_", "_,_,_,_,_,_", "_,_,_,_,_,_" };
            //_dict.Frequency = 6;
            //Dict.Add(_dict);
            //_dict.Str = "tset";

            ////arr[n]: Style Weight CharOffset Family Size Color
            //_dict.Formatting = new string[5] { "i,b,1,Arial,12,Black", "_,_,_,_,_,_", "n,n,_,_,_,_", "_,_,_,_,_,_", "_,n,_,_,_,_" };
            //_dict.Frequency = 4;
            //Dict.Add(_dict);

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
        public void jsonToDict(string fileName)
        {
            string jsonStr = File.ReadAllText(fileName);
            Dict = JsonSerializer.Deserialize<List<DictClass>>(jsonStr)!;
            //System.Diagnostics.Debug.WriteLine(Dict[0].Str);
            //System.Diagnostics.Debug.WriteLine(Dict[0].Frequency);
            //foreach(string s in Dict[0].Formatting)
            //{
            //    System.Diagnostics.Debug.WriteLine(s);
            //}
            
        }

        public void dictToJson()
        {
            try
            {
                dictToJson(DictFileName);
            }
            catch(System.ArgumentNullException e)
            {

            }
                            
        }

        public void jsonToDict()
        {
            jsonToDict(DictFileName);
            
        }
        #endregion

    }
}
