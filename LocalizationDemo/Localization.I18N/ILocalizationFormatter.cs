using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace Localization.I18N
{
    internal interface ILocalizationFormatter
    {
        public static ILocalizationFormatter ArgsString { get; set; } = new ArgsStringFormatter();
        public static ILocalizationFormatter Pseudo { get; set; } = new PseudoFormatter();

        string Format(string originString, params string[] formatParams);
    }

    internal class ArgsStringFormatter : ILocalizationFormatter
    {
        public string Format(string originString, params string[] formatParams)
        {
            var pattern = @"\{[^\}]+\}";
            var matches = Regex.Matches(originString, pattern);
            if (matches.Count == 0)
            {
                return originString;
            }
            else
            {
                var splits = Regex.Split(originString, pattern);
                if (splits.Length - formatParams.Length == 1)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < splits.Length; i++)
                    {
                        stringBuilder.Append(splits[i]);
                        if (i != splits.Length - 1)
                        {
                            stringBuilder.Append(formatParams[i]);
                        }
                    }
                    return stringBuilder.ToString();
                }
                else
                {
                    return originString;
                }
            }
        }
    }

    internal class PseudoFormatter : ILocalizationFormatter
    {
        private readonly Dictionary<char, char> pseudoCharMap = new Dictionary<char, char>();

        public PseudoFormatter()
        {
            pseudoCharMap = new Dictionary<char, char>
            {
                { 'a', 'ä' },
                { 'b', 'ƃ' },
                { 'c', 'č' },
                { 'd', 'ƌ' },
                { 'e', 'ë' },
                { 'f', 'ƒ' },
                { 'g', 'ğ' },
                { 'h', 'ħ' },
                { 'i', 'ï' },
                { 'j', 'ĵ' },
                { 'k', 'ƙ' },
                { 'l', 'ł' },
                { 'm', 'ɱ' },
                { 'n', 'ň' },
                { 'o', 'ö' },
                { 'p', 'þ' },
                { 'q', 'ɋ' },
                { 'r', 'ř' },
                { 's', 'š' },
                { 't', 'ŧ' },
                { 'u', 'ü' },
                { 'v', 'ṽ' },
                { 'w', 'ŵ' },
                { 'x', 'ӿ' },
                { 'y', 'ŷ' },
                { 'z', 'ž' },
                { 'A', 'Ä' },
                { 'B', 'Ɓ' },
                { 'C', 'Č' },
                { 'D', 'Đ' },
                { 'E', 'Ë' },
                { 'F', 'Ƒ' },
                { 'G', 'Ğ' },
                { 'H', 'Ħ' },
                { 'I', 'Ï' },
                { 'J', 'Ĵ' },
                { 'K', 'Ҟ' },
                { 'L', 'Ł' },
                { 'M', 'Ӎ' },
                { 'N', 'Ň' },
                { 'O', 'Ö' },
                { 'P', 'Ҏ' },
                { 'Q', 'Ǫ' },
                { 'R', 'Ř' },
                { 'S', 'Š' },
                { 'T', 'Ŧ' },
                { 'U', 'Ü' },
                { 'V', 'Ṽ' },
                { 'W', 'Ŵ' },
                { 'X', 'Ӿ' },
                { 'Y', 'Ŷ' },
                { 'Z', 'Ž' },
            };
        }

        public string Format(string originString, params string[] formatParams)
        {
            var pseudoString = string.Join("", originString.Select(c => pseudoCharMap.ContainsKey(c) ? pseudoCharMap[c] : c));
            pseudoString = string.Join("", pseudoString.Select((c, i) => (i % 3 == 0) ? $"{c}_" : c.ToString()));
            return pseudoString;
        }
    }
}
