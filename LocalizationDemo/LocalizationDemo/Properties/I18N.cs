using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            [Description("MessageBox")] MessageBox,
            [Description("Show New Window")] ShowNewWindow,
            [Description("newWindowTitle")] NewWindowTitle,
            [Description("Success")] Success,
            [Description("this is string start")] String1,
            [Description("this is string end")] String2,
        }

        class BindingData : I18NWeakEventListenerAbstract
        {
            private readonly I18NKeys key;
            private readonly Action<string> actionWithArg;
            private readonly Action action;

            public BindingData(I18NKeys key, Action<string> actionWithArg)
            {
                this.key = key;
                this.actionWithArg = actionWithArg;
                ReceiveWeakEvent();
            }
            public BindingData(Action action)
            {
                this.action = action;
                ReceiveWeakEvent();
            }
            ~BindingData()
            {
            }

            #region I18NWeakEventListenerAbstract

            public override void ReceiveWeakEvent()
            {
                actionWithArg?.Invoke(key.GetLocalizationString());
                action?.Invoke();
            }

            #endregion
        }

        private const char SPLIT = ':';

        public static event EventHandler CultureChanged;

        private static Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();
        private static Dictionary<object, List<BindingData>> bindingDataMap = new Dictionary<object, List<BindingData>>();

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

        public static void BindingLocalizationString(this I18NKeys i18NKeys, object sender, Action<string> action)
        {
            if (!bindingDataMap.TryGetValue(sender, out List<BindingData> bindingDatas))
            {
                bindingDatas = new List<BindingData>();
                bindingDataMap.Add(sender, bindingDatas);
            }
            bindingDatas.Add(new BindingData(i18NKeys, action));
        }

        public static void BindingLocalizationString(object sender, Action action)
        {
            if (!bindingDataMap.TryGetValue(sender, out List<BindingData> bindingDatas))
            {
                bindingDatas = new List<BindingData>();
                bindingDataMap.Add(sender, bindingDatas);
            }
            bindingDatas.Add(new BindingData(action));
        }

        public static void RemoveBinding(object sender)
        {
            bindingDataMap.Remove(sender);
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
