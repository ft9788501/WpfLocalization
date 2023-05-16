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
            var pattern = @"\{l_[^\}]+\}";
            var matches = Regex.Matches(originString, pattern);
            var enablePseudo = I18NManager.EnablePseudo;
            if (matches.Count == 0)
            {
                return enablePseudo ? PseudoHelper.GetPseudoString(originString) : originString;
            }
            else
            {
                bool isParamsValid = matches.Count == formatParams.Length;
                var splits = Regex.Split(originString, pattern);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < splits.Length; i++)
                {
                    stringBuilder.Append(enablePseudo ? PseudoHelper.GetPseudoString(splits[i]) : splits[i]);
                    if (i != splits.Length - 1)
                    {
                        stringBuilder.Append(isParamsValid ? formatParams[i] : matches[i].Value);
                    }
                }
                return stringBuilder.ToString();
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
            var enablePseudo = I18NManager.EnablePseudo;
            if (matches.Count == 0)
            {
                yield return enablePseudo ? PseudoHelper.GetPseudoString(originString) : originString;
            }
            else
            {
                bool isParamsValid = matches.Count == formatParams.Length;
                var splits = Regex.Split(originString, pattern);
                for (int i = 0; i < splits.Length; i++)
                {
                    yield return enablePseudo ? PseudoHelper.GetPseudoString(splits[i]) : splits[i];
                    if (i != splits.Length - 1)
                    {
                        yield return isParamsValid ? formatParams[i] : matches[i].Value;
                    }
                }
            }
        }
    }
}
