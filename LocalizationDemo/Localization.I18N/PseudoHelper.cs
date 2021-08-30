using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Localization.I18N
{
    internal static class PseudoHelper
    {
        private static readonly Dictionary<char, char> pseudoCharMap = new Dictionary<char, char>
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

        internal static string GetPseudoString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
            {
                return rawString;
            }
            var pseudoString = string.Join("", rawString.Select(c => pseudoCharMap.ContainsKey(c) ? pseudoCharMap[c] : c));
            pseudoString = string.Join("", pseudoString.Select((c, i) => (i % 3 == 0) ? $"{c}_" : c.ToString()));
            return $"{pseudoString}";
        }
    }
}
