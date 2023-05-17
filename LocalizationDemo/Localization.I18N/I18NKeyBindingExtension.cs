using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Localization.I18N
{
    public class I18NKeyBindingExtension : MarkupExtension
    {
        abstract class BindingArgument
        {
            public class ValueArgument : BindingArgument
            {
                public override object Value { get; }

                public ValueArgument(object value)
                {
                    Value = value;
                }
            }

            //public class MarkupArgument : BindingArgument
            //{
            //    private MarkupReader markupReader;

            //    public override object Value => markupReader.Value;

            //    public MarkupArgument(AvaloniaObject avaloniaObject, MarkupExtension markup)
            //    {
            //        markupReader = new MarkupReader(avaloniaObject, markup);
            //    }
            //}

            public abstract object Value { get; }

            ~BindingArgument()
            {

            }
        }
        class BindingData : CultureChangedReceiverAbstract, INotifyPropertyChanged
        {
            private readonly I18NKeys key;
            private IEnumerable<BindingArgument> bindingArgs;

            public BindingData(I18NKeys key, IEnumerable<BindingArgument> bindingArgs, AvaloniaObject avaloniaObject)
            {
                this.key = key;
                this.bindingArgs = bindingArgs;
                if (avaloniaObject != null)
                {
                    //Binding.AddTargetUpdatedHandler(avaloniaObject, (s, e) =>
                    //{
                    //    OnCultureChanged();
                    //});
                    //Binding.AddSourceUpdatedHandler(avaloniaObject, (s, e) =>
                    //{
                    //    OnCultureChanged();
                    //});
                }
            }
            ~BindingData()
            {
            }

            public string Value
            {
                get
                {
                    var value = key.GetLocalizationString(bindingArgs.Select(b => (b.Value ?? string.Empty).ToString()).ToArray());
                    return value;
                }
            }

            #region INotifyPropertyChanged

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            #region CultureChangedReceiverAbstract

            public override void OnCultureChanged()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }

            #endregion
        }

        private readonly object[] args;

        public I18NKeyBindingExtension() : this(new object[0]) { }
        public I18NKeyBindingExtension(object arg) : this(new object[] { arg }) { }
        public I18NKeyBindingExtension(object arg0, object arg1) : this(new object[] { arg0, arg1 }) { }
        public I18NKeyBindingExtension(object arg0, object arg1, object arg2) : this(new object[] { arg0, arg1, arg2 }) { }
        public I18NKeyBindingExtension(object arg0, object arg1, object arg2, object arg3) : this(new object[] { arg0, arg1, arg2, arg3 }) { }
        public I18NKeyBindingExtension(object arg0, object arg1, object arg2, object arg3, object arg4) : this(new object[] { arg0, arg1, arg2, arg3, arg4 }) { }
        private I18NKeyBindingExtension(object[] args)
        {
            this.args = args;
        }
        ~I18NKeyBindingExtension()
        {
        }
        [ConstructorArgument(nameof(Key))]
        public I18NKeys Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            AvaloniaObject avaloniaObject = null;
            List<BindingArgument> bindingArgs = new List<BindingArgument>();
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget &&
                provideValueTarget.TargetObject is AvaloniaObject targetObject &&
                provideValueTarget.TargetProperty is AvaloniaProperty targetProperty)
            {
                avaloniaObject = targetObject;
                foreach (var arg in args)
                {
                    if (arg is Binding b)
                    {
                        targetObject.Bind(targetProperty, b);
                        var bb = targetObject.GetValue(targetProperty);

                    }
                    else if (arg is MarkupExtension markup)
                    {
                        //bindingArgs.Add(new BindingArgument.MarkupArgument(avaloniaObject, markup));
                    }
                    else
                    {
                        bindingArgs.Add(new BindingArgument.ValueArgument(arg));
                    }
                }
            }
            var binding = new ReflectionBindingExtension(nameof(BindingData.Value))
            {
                Source = new BindingData(Key, bindingArgs, avaloniaObject),
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}
