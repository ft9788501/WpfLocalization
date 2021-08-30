using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Localization.I18N
{
    public static class I18NKeysExtensions
    {
        class BindingExpressionData : CultureChangedWeakEventListenerAbstract
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
                if (I18NManager.EnablePseudo)
                {
                    rawString = PseudoHelper.GetPseudoString(rawString);
                }
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
                if (I18NManager.EnablePseudo)
                {
                    blockStrings = blockStrings.Select(b => PseudoHelper.GetPseudoString(b));
                }
            }
            return blockStrings;
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
                if (bindingExpressionData.Key != i18NKey)
                {
                    bindingExpressionData.Key = i18NKey;
                }
                bindingExpressionData.ReceiveWeakEvent();
            }
        }

    }
}
