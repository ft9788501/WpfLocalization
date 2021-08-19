using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace LocalizationDemo.Properties
{
    public static class I18N
    {
        public enum I18NKeys
        {
            [Description("title")] Title,
            [Description("name")] Name,
            [Description("12")] Age,
            [Description("Export Json")] ExportJson,
            [Description("Show New Window")] ShowNewWindow,
            [Description("newWindowTitle")] NewWindowTitle,
            [Description("Success")] Success
        }

        private const char SPLIT = ':';

        public static event EventHandler CultureChanged;

        private static Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();

        static I18N()
        {
            var properties = Enum.GetValues(typeof(I18NKeys))
                 .Cast<object>()
                 .Select(e => new KeyValuePair<I18NKeys, string>((I18NKeys)e, e.GetType().GetField(e.ToString()).GetCustomAttribute<DescriptionAttribute>()?.Description));
            Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();
            foreach (var property in properties)
            {
                i18nMap.Add(property.Key, property.Value);
            }
            I18N.i18nMap = i18nMap;
        }

        public static string GetLocalizationString(this I18NKeys i18NKeys)
        {
            return i18nMap[i18NKeys];
        }
        public static void BindingLocalizationString(this I18NKeys i18NKeys, Action<string> action)
        {
            var value = i18nMap[i18NKeys];
            action?.Invoke(value);
        }

        public static bool SaveAsJson(string path)
        {
            try
            {
                var properties = Enum.GetValues(typeof(I18NKeys))
                     .Cast<object>()
                     .Select(e => e.GetType().GetField(e.ToString()))
                     .Select(f => $"{f.Name}{SPLIT}{ f.GetCustomAttribute<DescriptionAttribute>()?.Description}").ToArray();
                var json = JsonSerializer.Serialize(properties);
                File.WriteAllText(path, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool LoadFromJson(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            try
            {
                var json = File.ReadAllText(path);
                var jsonStr = JsonSerializer.Deserialize<string[]>(json);
                Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();
                foreach (var propertyStr in jsonStr)
                {
                    var stringParts = propertyStr.Split(SPLIT);
                    var propertyName = stringParts[0];
                    var propertyValue = stringParts[1];
                    i18nMap.Add(Enum.Parse<I18NKeys>(propertyName), propertyValue);
                }
                I18N.i18nMap = i18nMap;
                CultureChanged?.Invoke(null, EventArgs.Empty);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
