using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Localization.I18N
{
    public static class I18NManager
    {
        public static event EventHandler<CultureInfo> CultureChanged;

        private static bool enablePseudo = false;
        private static CultureInfo currentCulture = null;

        private static readonly Dictionary<CultureInfo, CultureInfo> defaultCultureMap = new Dictionary<CultureInfo, CultureInfo>
        {
        };
        internal static Dictionary<I18NKeys, I18NValue> nonLocalizedMap = new Dictionary<I18NKeys, I18NValue>();
        internal static Dictionary<I18NKeys, I18NValue> i18nMapDefault = new Dictionary<I18NKeys, I18NValue>();
        internal static Dictionary<I18NKeys, I18NValue> i18nMap = new Dictionary<I18NKeys, I18NValue>();

        public static bool EnablePseudo
        {
            get => enablePseudo;
            set
            {
                enablePseudo = value;
                OnCultureChanged();
            }
        }
        public static CultureInfo CurrentCulture
        {
            get => currentCulture;
            set
            {
                Trace.WriteLine($"Set current culture {value.Name}");
                value = FixCultureInfo(value);
                Trace.WriteLine($"Set fixed culture {value.Name}");
                if (currentCulture?.Name == value?.Name)
                {
                    return;
                }
                currentCulture = value;
                using Stream nonLocalizedJsonStream = GetStreamByCultureName("non-localized");
                using Stream defaultCultureJsonStream = I18NManager.GetStreamByCultureName("en");
                using Stream cultureJsonStream = GetStreamByCultureName(currentCulture.Name);
                using StreamReader nonLocalizedJsonStreamReader = new StreamReader(nonLocalizedJsonStream, Encoding.UTF8);
                using StreamReader defaultCultureJsonStreamReader = new StreamReader(defaultCultureJsonStream, Encoding.UTF8);
                using StreamReader cultureJsonStreamReader = new StreamReader(cultureJsonStream, Encoding.UTF8);
                string nonLocalizedJson = nonLocalizedJsonStreamReader.ReadToEnd();
                string defaultCultureJson = defaultCultureJsonStreamReader.ReadToEnd();
                string cultureJson = cultureJsonStreamReader.ReadToEnd();
                LoadFromJson(nonLocalizedJson, defaultCultureJson, cultureJson);
                OnCultureChanged();
            }
        }

        public static IEnumerable<CultureInfo> SupportCultureList { get; } = EmbeddedCultureFileList
            .Where(x => x != $"{nameof(Localization)}.I18N.I18NResources.non-localized.json")
            .Select(x => x.Replace($"{nameof(Localization)}.I18N.I18NResources.", ""))
            .Select(x => Path.GetFileNameWithoutExtension(x))
            .Select(x => CultureInfo.GetCultureInfo(x));

        private static IEnumerable<string> EmbeddedCultureFileList => Assembly.GetExecutingAssembly()
            .GetManifestResourceNames()
            .Where(x => x.Contains($"{nameof(Localization)}.I18N.I18NResources"))
            .Where(x => Path.GetExtension(x) == ".json");

        static I18NManager()
        {
            Trace.WriteLine($"I18NManager support cultures {string.Join("\r\n", SupportCultureList.Select(x => x.Name))}");
            Trace.WriteLine($"I18NManager init {CultureInfo.CurrentCulture.Name}");
            CurrentCulture = CultureInfo.CurrentCulture;
        }

        private static Stream GetStreamByCultureName(string cultureName)
        {
            var manifestResourcePath = EmbeddedCultureFileList.FirstOrDefault(x => x.Replace($"{nameof(Localization)}.I18N.I18NResources.", "") == $"{cultureName}.json");
            if (manifestResourcePath == null)
            {
                return null;
            }
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestResourcePath);
            return stream;
        }

        private static CultureInfo FixCultureInfo(CultureInfo culture)
        {
            using Stream cultureJsonStream = GetStreamByCultureName(culture.Name);
            if (cultureJsonStream == null)
            {
                Trace.WriteLine($"Fix culture {culture.Name}");
                if (defaultCultureMap.TryGetValue(culture, out CultureInfo sameCultureInfo))
                {
                    return FixCultureInfo(sameCultureInfo);
                }
                if (culture.Name == culture.TwoLetterISOLanguageName)
                {
                    return FixCultureInfo(CultureInfo.GetCultureInfo("en"));
                }
                return FixCultureInfo(CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName));
            }
            else
            {
                return culture;
            }
        }

        private static bool LoadFromJson(string nonLocalizedJson, string defaultCultureJson, string cultureJson)
        {
            try
            {
                nonLocalizedMap.Clear();
                i18nMapDefault.Clear();
                i18nMap.Clear();
                var nonLocalizedJsonDictionary = GetLocalizationMap(nonLocalizedJson);
                var defaultCultureJsonDictionary = GetLocalizationMap(defaultCultureJson);
                var cultureJsonDictionary = GetLocalizationMap(cultureJson);
                Trace.WriteLine($"Load from json culture [nonLocalized:{nonLocalizedJsonDictionary.Count()}|culture:{cultureJsonDictionary.Count()}]");
                foreach (var property in nonLocalizedJsonDictionary)
                {
                    var key = $"NonLocalized_{property.Key}";
                    if (Enum.TryParse(key, out I18NKeys i18NKey) && I18NValue.CreateI18NValue(property) is I18NValue value)
                    {
                        nonLocalizedMap.Add(i18NKey, value);
                    }
                }
                foreach (var property in defaultCultureJsonDictionary)
                {
                    var key = property.Key;
                    if (Enum.TryParse(key, out I18NKeys i18NKey) && I18NValue.CreateI18NValue(property) is I18NValue value)
                    {
                        i18nMapDefault.Add(i18NKey, value);
                    }
                }
                foreach (var property in cultureJsonDictionary)
                {
                    var key = property.Key;
                    if (Enum.TryParse(key, out I18NKeys i18NKey) && I18NValue.CreateI18NValue(property) is I18NValue value)
                    {
                        i18nMap.Add(i18NKey, value);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> GetLocalizationMap(string? jsonContent, string? parentKey = null)
        {
            if (jsonContent != null)
            {
                var jsonObjects = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
                if (jsonObjects != null)
                {
                    foreach (var jsonObject in jsonObjects)
                    {
                        var key = (parentKey == null ? "" : $"{parentKey}_") + jsonObject.Key;
                        if (jsonObject.Key == "Plurals" && parentKey != null)
                        {
                            var plurals = Plurals.LoadFromJson(jsonObject.Value.ToString());
                            if (plurals != null)
                            {
                                yield return new KeyValuePair<string, object>(parentKey, plurals);
                            }
                            continue;
                        }
                        if (jsonObject.Value is string value)
                        {
                            yield return new KeyValuePair<string, object>(key, value);
                            continue;
                        }
                        foreach (var item in GetLocalizationMap(jsonObject.Value.ToString(), key))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        private static void OnCultureChanged()
        {
            CultureChanged?.Invoke(null, currentCulture);
        }
    }
}
