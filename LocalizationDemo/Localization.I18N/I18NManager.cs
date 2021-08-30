using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Resources;

namespace Localization.I18N
{
    public static class I18NManager
    {
        public static event EventHandler<CultureInfo> CultureChanged;

        private static bool enablePseudo = false;
        private static CultureInfo currentCulture = null;

        private static readonly Dictionary<CultureInfo, CultureInfo> defaultCultureMap = new Dictionary<CultureInfo, CultureInfo>
        {
            { CultureInfo.GetCultureInfo("zh-HK"), CultureInfo.GetCultureInfo("zh-TW") },
            { CultureInfo.GetCultureInfo("en-AU"), CultureInfo.GetCultureInfo("en-GB") },
            { CultureInfo.GetCultureInfo("en"), CultureInfo.GetCultureInfo("en-US") },
            { CultureInfo.GetCultureInfo("de"), CultureInfo.GetCultureInfo("de-DE") },
            { CultureInfo.GetCultureInfo("es"), CultureInfo.GetCultureInfo("es-419") },
            { CultureInfo.GetCultureInfo("fr"), CultureInfo.GetCultureInfo("fr-FR") },
            { CultureInfo.GetCultureInfo("it"), CultureInfo.GetCultureInfo("it-IT") },
            { CultureInfo.GetCultureInfo("ja"), CultureInfo.GetCultureInfo("ja-JP") },
            { CultureInfo.GetCultureInfo("pt"), CultureInfo.GetCultureInfo("pt-BR") },
            { CultureInfo.GetCultureInfo("zh"), CultureInfo.GetCultureInfo("zh-CN") },
            { CultureInfo.GetCultureInfo("nl"), CultureInfo.GetCultureInfo("nl-NL") },
            { CultureInfo.GetCultureInfo("ko"), CultureInfo.GetCultureInfo("ko-KR") },
        };
        internal static Dictionary<I18NKeys, I18NValue> nonLocalizedMap = new Dictionary<I18NKeys, I18NValue>();
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
                value = FixCultureInfo(value);
                if (currentCulture?.Name == value?.Name)
                {
                    return;
                }
                currentCulture = value;
                using Stream nonLocalizedJsonStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(Localization)}.{nameof(I18N)}.I18NResources.non-localized.json");
                using Stream cultureJsonStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(Localization)}.{nameof(I18N)}.I18NResources.{value.Name}.json");
                using StreamReader nonLocalizedJsonStreamReader = new StreamReader(nonLocalizedJsonStream, Encoding.UTF8);
                using StreamReader cultureJsonStreamReader = new StreamReader(cultureJsonStream, Encoding.UTF8);
                string nonLocalizedJson = nonLocalizedJsonStreamReader.ReadToEnd();
                string cultureJson = cultureJsonStreamReader.ReadToEnd();
                LoadFromJson(nonLocalizedJson, cultureJson);
            }
        }
        public static IEnumerable<CultureInfo> SupportCultureList
        {
            get
            {
                yield return CultureInfo.GetCultureInfo("en-US");
                yield return CultureInfo.GetCultureInfo("it-IT");
                yield return CultureInfo.GetCultureInfo("pt-BR");
                yield return CultureInfo.GetCultureInfo("zh-CN");
                yield return CultureInfo.GetCultureInfo("zh-TW");
                yield return CultureInfo.GetCultureInfo("de-DE");
                yield return CultureInfo.GetCultureInfo("fr-FR");
                yield return CultureInfo.GetCultureInfo("fr-CA");
                yield return CultureInfo.GetCultureInfo("es-ES");
                yield return CultureInfo.GetCultureInfo("es-419");
                yield return CultureInfo.GetCultureInfo("ja-JP");
                yield return CultureInfo.GetCultureInfo("en-GB");
                yield return CultureInfo.GetCultureInfo("zh-HK");
                yield return CultureInfo.GetCultureInfo("en-AU");
                yield return CultureInfo.GetCultureInfo("ko-KR");
                yield return CultureInfo.GetCultureInfo("nl-NL");
                yield return CultureInfo.GetCultureInfo("pt-PT");
                yield return CultureInfo.GetCultureInfo("fi-FI");
            }
        }

        static I18NManager()
        {
            CurrentCulture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// https://docs.google.com/spreadsheets/d/1P5hXOTJyiBR1WFW9h967ilztUvuZtBhaY91bt432uH4/edit#gid=15186963
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        private static CultureInfo FixCultureInfo(CultureInfo culture)
        {
            using Stream cultureJsonStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(Localization)}.{nameof(I18N)}.I18NResources.{culture.Name}.json");
            if (cultureJsonStream == null)
            {
                if (defaultCultureMap.TryGetValue(culture, out CultureInfo sameCultureInfo))
                {
                    return FixCultureInfo(sameCultureInfo);
                }
                else if (defaultCultureMap.TryGetValue(CultureInfo.GetCultureInfo(culture.TwoLetterISOLanguageName), out CultureInfo sameParentCultureInfo) && culture.Name != sameParentCultureInfo.Name)
                {
                    return FixCultureInfo(sameParentCultureInfo);
                }
                else
                {
                    return FixCultureInfo(CultureInfo.GetCultureInfo("en-US"));
                }
            }
            else
            {
                return culture;
            }
        }

        private static bool LoadFromJson(string nonLocalizedJson, string cultureJson)
        {
            try
            {
                Dictionary<I18NKeys, I18NValue> nonLocalizedMap = new Dictionary<I18NKeys, I18NValue>();
                Dictionary<I18NKeys, I18NValue> i18nMap = new Dictionary<I18NKeys, I18NValue>();
                var nonLocalizedJsonDictionary = JsonSerializer.Deserialize<Dictionary<string, object>[]>(nonLocalizedJson);
                var cultureJsonDictionary = JsonSerializer.Deserialize<Dictionary<string, object>[]>(cultureJson);
                foreach (var property in nonLocalizedJsonDictionary)
                {
                    var key = property["Key"].ToString();
                    if (I18NValue.CreateI18NValue(property) is I18NValue value)
                    {
                        nonLocalizedMap.Add(Enum.Parse<I18NKeys>(key), value);
                    }
                }
                I18NManager.nonLocalizedMap = nonLocalizedMap;
                foreach (var property in cultureJsonDictionary)
                {
                    var key = property["Key"].ToString();
                    if (I18NValue.CreateI18NValue(property) is I18NValue value)
                    {
                        i18nMap.Add(Enum.Parse<I18NKeys>(key), value);
                    }
                }
                I18NManager.i18nMap = i18nMap;
                OnCultureChanged();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void OnCultureChanged()
        {
            CultureChanged?.Invoke(null, currentCulture);
        }
    }
}
