using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Localization.I18N
{
    public static class I18NKeysExtensions
    {
        class BindingExpressionData : CultureChangedReceiverAbstract
        {
            private readonly object sender;
            private readonly PropertyInfo propertyInfo;

            public I18NKeys Key { get; set; }
            public string[] FormatParams { get; set; }

            public BindingExpressionData(I18NKeys key, object sender, PropertyInfo propertyInfo, params string[] formatParams)
            {
                Key = key;
                this.sender = sender;
                this.propertyInfo = propertyInfo;
                FormatParams = formatParams;
            }
            ~BindingExpressionData()
            {
            }

            public bool Equals(object sender, PropertyInfo propertyInfo)
            {
                return this.sender == sender && this.propertyInfo == propertyInfo;
            }

            #region CultureChangedReceiverAbstract

            public override void OnCultureChanged()
            {
                propertyInfo?.SetValue(sender, Key.GetLocalizationString(FormatParams));
                //if (sender is AvaloniaObject avaloniaObject)
                //{
                //    avaloniaObject.Dispatcher.Invoke(() =>
                //    {
                //        propertyInfo?.SetValue(sender, Key.GetLocalizationString(FormatParams));
                //    });
                //}
                //else
                //{
                //    propertyInfo?.SetValue(sender, Key.GetLocalizationString(FormatParams));
                //}
            }

            #endregion
        }

        private static readonly ConditionalWeakTable<object, List<BindingExpressionData>> bindingExpressionMap = new ConditionalWeakTable<object, List<BindingExpressionData>>();

        public static string GetLocalizationString(this I18NKeys i18NKey, params string[] formatParams)
        {
            string rawString;
            if (I18NManager.nonLocalizedMap.ContainsKey(i18NKey))
            {
                rawString = I18NManager.nonLocalizedMap[i18NKey].GetMultiConditionValue(out string[] convertedParams, formatParams);
                formatParams = convertedParams;
                rawString = StringFormatterHelper.Format(rawString, formatParams);
            }
            else
            {
                rawString = I18NManager.i18nMap[i18NKey].GetMultiConditionValue(out string[] convertedParams, formatParams);
                formatParams = convertedParams;
                rawString = StringFormatterHelper.Format(rawString, formatParams);
            }
            return rawString;
        }

        /// <summary>
        /// Get the blocks that localization formated return
        /// </summary>
        /// <param name="i18NKey">"Camera {old camera device name} is unavailable. Using {new camera device name}"</param>
        /// <param name="formatParams">new string[] { "Old Device", "New Device" }</param>
        /// <example>
        /// <code>
        /// return
        /// {
        ///     yield return "Camera ";
        ///     yield return "Old Device";
        ///     yield return " is unavailable. Using ";
        ///     yield return "New Device";
        ///     yield return "";
        /// }
        /// </code>
        /// </example>
        /// <returns>
        /// IEnumerable<string>
        /// </returns>
        public static IEnumerable<string> GetLocalizationBlock(this I18NKeys i18NKey, params string[] formatParams)
        {
            IEnumerable<string> blockStrings;
            if (I18NManager.nonLocalizedMap.ContainsKey(i18NKey))
            {
                var rawString = I18NManager.nonLocalizedMap[i18NKey].GetMultiConditionValue(out string[] convertedParams, formatParams);
                formatParams = convertedParams;
                blockStrings = StringFormatterHelper.FormatBlock(rawString, formatParams);
            }
            else
            {
                var rawString = I18NManager.i18nMap[i18NKey].GetMultiConditionValue(out string[] convertedParams, formatParams);
                formatParams = convertedParams;
                blockStrings = StringFormatterHelper.FormatBlock(rawString, formatParams);
            }
            return blockStrings;
        }

        public static string GetLocalizationRawValue(this I18NKeys i18NKey)
        {
            string rawString;
            if (I18NManager.nonLocalizedMap.ContainsKey(i18NKey))
            {
                rawString = I18NManager.nonLocalizedMap[i18NKey].GetAllValues();
            }
            else
            {
                rawString = I18NManager.i18nMap[i18NKey].GetAllValues();
            }
            return rawString;
        }

        /// <summary>
        /// Use this binding in C# code
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i18NKey"></param>
        /// <param name="sender"></param>
        /// <param name="memberLambda"></param>
        /// <param name="formatParams"></param>
        /// <example>
        /// <code>
        /// I18NKeys.MsgWaitSignInCotroller.BindingExpression(PairingFlowDataCenter.Instance, x => x.SignFlowMessage);
        /// </code>
        /// </example>
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
                else
                {
                    if (bindingExpressionData.Key != i18NKey)
                    {
                        bindingExpressionData.Key = i18NKey;
                    }
                    if (bindingExpressionData.FormatParams != formatParams)
                    {
                        bindingExpressionData.FormatParams = formatParams;
                    }
                }
                bindingExpressionData.OnCultureChanged();
            }
        }

    }
}
