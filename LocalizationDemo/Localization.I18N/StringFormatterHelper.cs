using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Localization.I18N
{
    internal static class StringFormatterHelper
    {
        public static string Format(string originString, params string[] formatParams)
        {
            if (string.IsNullOrEmpty(originString))
            {
                return originString;
            }
            var pattern = @"\{rcrooms_[^\}]+\}";
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

        public static IEnumerable<string> FormatBlock(string originString, params string[] formatParams)
        {
            if (string.IsNullOrEmpty(originString))
            {
                yield return originString;
            }
            var pattern = @"\{rcrooms_[^\}]+\}";
            var matches = Regex.Matches(originString, pattern);
            if (matches.Count == 0)
            {
                yield return originString;
            }
            else
            {
                var splits = Regex.Split(originString, pattern);
                if (splits.Length - formatParams.Length == 1)
                {
                    for (int i = 0; i < splits.Length; i++)
                    {
                        yield return splits[i];
                        if (i != splits.Length - 1)
                        {
                            yield return formatParams[i];
                        }
                    }
                }
                else
                {
                    yield return originString;
                }
            }
        }
    }
}
