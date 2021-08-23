using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Localization.I18N
{
    public class I18NExtension : MarkupExtension
    {
        class BindingData : WeakEventListenerAbstract, INotifyPropertyChanged
        {
            private readonly I18NKeys key;

            public BindingData(I18NKeys key)
            {
                this.key = key;
            }
            ~BindingData()
            {
            }

            public string Value
            {
                get
                {
                    return I18NManager.GetLocalizationString(key);
                }
            }

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region I18NWeakEventListenerAbstract

            public override void ReceiveWeakEvent()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }

            #endregion
        }

        [ConstructorArgument(nameof(Key))]
        public I18NKeys Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding(nameof(BindingData.Value))
            {
                Source = new BindingData(Key),
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}
