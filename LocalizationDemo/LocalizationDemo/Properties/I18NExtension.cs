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
        class CultureChangedEventManager : WeakEventManager
        {
            private static CultureChangedEventManager CurrentManager
            {
                get
                {
                    CultureChangedEventManager manager = (CultureChangedEventManager)GetCurrentManager(typeof(CultureChangedEventManager));
                    if (manager == null)
                    {
                        manager = new CultureChangedEventManager();
                        SetCurrentManager(typeof(CultureChangedEventManager), manager);
                    }
                    return manager;
                }
            }

            public static void AddListener(IWeakEventListener listener)
            {
                CurrentManager.ProtectedAddListener(null, listener);
            }

            public static void RemoveListener(IWeakEventListener listener)
            {
                CurrentManager.ProtectedRemoveListener(null, listener);
            }

            protected override void StartListening(object source)
            {
                I18N.CultureChanged += OnCultureChanged;
            }

            protected override void StopListening(object source)
            {
                I18N.CultureChanged -= OnCultureChanged;
            }

            private void OnCultureChanged(object sender, EventArgs e)
            {
                DeliverEvent(null, e);
            }
        }
        class BindingData : INotifyPropertyChanged, IWeakEventListener
        {
            private readonly I18N.I18NKeys key;

            public BindingData(I18N.I18NKeys key)
            {
                this.key = key;
                CultureChangedEventManager.AddListener(this);
            }

            ~BindingData()
            {
                CultureChangedEventManager.RemoveListener(this);
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

            #region IWeakEventListener

            public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                if (managerType == typeof(CultureChangedEventManager))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    return true;
                }
                return false;
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
