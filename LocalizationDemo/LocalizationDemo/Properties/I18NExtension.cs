using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace LocalizationDemo.Properties
{
    public class I18NExtension : MarkupExtension
    {
        class BindingData : I18NWeakEventListenerAbstract, INotifyPropertyChanged
        {
            private readonly I18N.I18NKeys key;

            public BindingData(I18N.I18NKeys key)
            {
                this.key = key;
            }

            public string Value
            {
                get
                {
                    return I18N.GetLocalizationString(key);
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
        public I18N.I18NKeys Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding(nameof(BindingData.Value))
            {
                Source = new BindingData(Key)
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}
