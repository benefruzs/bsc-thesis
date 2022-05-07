using System;
using System.Collections.Generic;
using System.Linq;
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
        #endregion

        #region Private methods

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

        private char offsetToChar(int offset)
        {
            switch (offset)
            {
                case 10:
                    return 'p';
                case -10:
                    return 'q';
                default:
                    return 'n';
            }
        }

        private int charToOffset(char offset)
        {
            switch (offset)
            {
                case 'p':
                    return 10;
                case 'q':
                    return -10;
                default:
                    return 0;
            }
        }

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
                styleFreq[ft_arr[i].Style]++;
                weightFreq[ft_arr[i].Weight]++;
                familyFreq[ft_arr[i].Family]++;
                chOffsetFreq[ft_arr[i].CharOffset]++;
                sizeFreq[ft_arr[i].Size]++;
                colorFreq[ft_arr[i].Color]++;
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
            //arr[0]: ami mindenre (sokra) vonatkozik, ha nincs ilyen, akkor '_'
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
            _dict = new DictClass(st.SelectedText.SelectedString, _format_array, 1);

        }

        #endregion

        #region Public methods
        public void toDictionary(Stwf str)
        {
            fromStwfToDict(str);
            //if the string, formatting and so already exists then update dictionary content
            //search + DictClass.Freq++
            var ind = Dict.FindIndex(x => x.Str == _dict.Str && x.Formatting.Equals(_dict.Formatting));
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

        public Stwf fromDictionary(Stwf str)
        {
            return null;
        }

        public void dictToJson()
        {

        }
        #endregion

    }
}
