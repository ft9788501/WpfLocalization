using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Resources;

namespace Localization.I18N
{
    public static class I18N
    {
        class BindingExpressionData : I18NWeakEventListenerAbstract
        {
            private readonly object sender;
            private readonly PropertyInfo propertyInfo;

            public I18NKeys Key { get; set; }

            public BindingExpressionData(I18NKeys key, object sender, PropertyInfo propertyInfo)
            {
                Key = key;
                this.sender = sender;
                this.propertyInfo = propertyInfo;
            }
            ~BindingExpressionData()
            {
            }

            public bool Equals(object sender, PropertyInfo propertyInfo)
            {
                return this.sender == sender && this.propertyInfo == propertyInfo;
            }

            #region I18NWeakEventListenerAbstract

            public override void ReceiveWeakEvent()
            {
                propertyInfo?.SetValue(sender, Key.GetLocalizationString());
            }

            #endregion
        }

        public static event EventHandler CultureChanged;

        private const char SPLIT = ':';
        private static Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();
        private static readonly ConditionalWeakTable<object, List<BindingExpressionData>> bindingExpressionMap = new ConditionalWeakTable<object, List<BindingExpressionData>>();

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

        public static void BindingExpression<T>(this I18NKeys i18NKey, T sender, Expression<Func<T, object>> memberLambda)
        {
            if (!bindingExpressionMap.TryGetValue(sender, out List<BindingExpressionData> bindingExpressionDatas))
            {
                bindingExpressionDatas = new List<BindingExpressionData>();
                bindingExpressionMap.Add(sender, bindingExpressionDatas);
            }
            var memberExpression = memberLambda.Body as MemberExpression;
            var property = memberExpression?.Member as PropertyInfo;
            if (property != null)
            {
                var bindingExpressionData = bindingExpressionDatas.FirstOrDefault(b => b.Equals(sender, property));
                if (bindingExpressionData == null)
                {
                    bindingExpressionData = new BindingExpressionData(i18NKey, sender, property);
                    bindingExpressionDatas.Add(bindingExpressionData);
                }
                if (bindingExpressionData.Key != i18NKey)
                {
                    bindingExpressionData.Key = i18NKey;
                }
                bindingExpressionData.ReceiveWeakEvent();
            }
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
                return LoadFromJsonStr(json);
            }
            catch
            {
                return false;
            }
        }

        public static bool LoadFromJsonStr(string json)
        {
            try
            {
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

        public static void SetCulture(string culture)
        {
            Uri uri = new Uri(@$"pack://application:,,,/Localization.I18N;component/I18NResources/{culture}.json", UriKind.Absolute);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            using (info.Stream)
            {
                using StreamReader streamReader = new StreamReader(info.Stream);
                string json = streamReader.ReadToEnd();
                LoadFromJsonStr(json);
            }
        }
    }
}
