using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Windows;
using System.Windows.Resources;

namespace Localization.I18N
{
    public static class I18NManager
    {
        class BindingExpressionData : WeakEventListenerAbstract
        {
            private readonly object sender;
            private readonly PropertyInfo propertyInfo;
            private readonly string[] formatParams;

            public I18NKeys Key { get; set; }

            public BindingExpressionData(I18NKeys key, object sender, PropertyInfo propertyInfo, params string[] formatParams)
            {
                Key = key;
                this.sender = sender;
                this.propertyInfo = propertyInfo;
                this.formatParams = formatParams;
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
                if (sender is DependencyObject dependencyObject)
                {
                    dependencyObject.Dispatcher.Invoke(() =>
                    {
                        propertyInfo?.SetValue(sender, Key.GetLocalizationString(formatParams));
                    });
                }
                else
                {
                    propertyInfo?.SetValue(sender, Key.GetLocalizationString(formatParams));
                }
            }

            #endregion
        }

        public static event EventHandler CultureChanged;

        private static bool enablePseudo = false;
        private static Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();
        private static readonly ConditionalWeakTable<object, List<BindingExpressionData>> bindingExpressionMap = new ConditionalWeakTable<object, List<BindingExpressionData>>();

        public static bool EnablePseudo
        {
            get => enablePseudo;
            set
            {
                enablePseudo = value;
                OnCultureChanged();
            }
        }

        private static IEnumerable<ILocalizationFormatter> Formatters
        {
            get
            {
                yield return ILocalizationFormatter.ArgsStringFormatter;
                if (EnablePseudo)
                {
                    yield return ILocalizationFormatter.PseudoFormatter;
                }
            }
        }

        static I18NManager()
        {
            var properties = Enum.GetValues(typeof(I18NKeys))
                 .Cast<object>()
                 .Select(e => new KeyValuePair<I18NKeys, string>((I18NKeys)e, e.GetType().GetField(e.ToString()).GetCustomAttribute<DescriptionAttribute>()?.Description));
            Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();
            foreach (var property in properties)
            {
                i18nMap.Add(property.Key, property.Value);
            }
            I18NManager.i18nMap = i18nMap;
        }

        public static string GetLocalizationString(this I18NKeys i18NKeys, params string[] formatParams)
        {
            return Format(i18nMap[i18NKeys], formatParams);
        }

        private static string Format(string originString, params string[] formatParams)
        {
            foreach (var formatter in Formatters)
            {
                originString = formatter.Format(originString, formatParams);
            }
            return originString;
        }

        public static void BindingExpression<T>(this I18NKeys i18NKey, T sender, Expression<Func<T, object>> memberLambda, params string[] formatParams)
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
                    bindingExpressionData = new BindingExpressionData(i18NKey, sender, property, formatParams);
                    bindingExpressionDatas.Add(bindingExpressionData);
                }
                if (bindingExpressionData.Key != i18NKey)
                {
                    bindingExpressionData.Key = i18NKey;
                }
                bindingExpressionData.ReceiveWeakEvent();
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
                var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>[]>(json);
                Dictionary<I18NKeys, string> i18nMap = new Dictionary<I18NKeys, string>();
                foreach (var property in dictionary)
                {
                    var key = property["Key"];
                    var value = property["Value"];
                    i18nMap.Add(Enum.Parse<I18NKeys>(key), value);
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

        public static void SetCulture(string culture)
        {
            Uri uri = new Uri(@$"pack://application:,,,/Localization.I18N;component/I18NResources/{culture}.json", UriKind.Absolute);
            StreamResourceInfo info = Application.GetResourceStream(uri);
            using (info.Stream)
            {
                using StreamReader streamReader = new StreamReader(info.Stream, Encoding.UTF8);
                string json = streamReader.ReadToEnd();
                LoadFromJsonStr(json);
            }
        }
        private static void OnCultureChanged()
        {
            CultureChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
