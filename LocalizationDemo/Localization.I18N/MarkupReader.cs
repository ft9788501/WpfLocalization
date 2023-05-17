using Avalonia;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Localization.I18N
{
    public class MarkupReader
    {
        private class ServiceProvider : IServiceProvider, IProvideValueTarget
        {
            public object TargetObject { get; }
            public object TargetProperty { get; }

            public ServiceProvider(object targetObject, object targetProperty)
            {
                TargetObject = targetObject;
                TargetProperty = targetProperty;
            }

            public object GetService(Type serviceType)
            {
                return serviceType.IsInstanceOfType(this) ? this : null;
            }
        }

        private static readonly ConditionalWeakTable<AvaloniaObject, List<AvaloniaProperty>> dependencyMap = new ConditionalWeakTable<AvaloniaObject, List<AvaloniaProperty>>();

        private readonly AvaloniaObject avaloniaObject;
        private readonly AvaloniaProperty avaloniaProperty;

        public object Value => avaloniaObject.GetValue(avaloniaProperty);

        public MarkupReader(AvaloniaObject avaloniaObject, MarkupExtension markup)
        {
            this.avaloniaObject = avaloniaObject;
            if (!dependencyMap.TryGetValue(avaloniaObject, out List<AvaloniaProperty> dependencyPropertys))
            {
                dependencyPropertys = new List<AvaloniaProperty>();
                dependencyMap.Add(avaloniaObject, dependencyPropertys);
            }
            avaloniaProperty = dependencyPropertys.FirstOrDefault(p => avaloniaObject.ReadLocalValue(p) == AvaloniaProperty.UnsetValue);
            if (avaloniaProperty == null)
            {
                avaloniaProperty = AvaloniaProperty.RegisterAttached($"{avaloniaObject.GetType().Name}{Guid.NewGuid()}",
                    typeof(object),
                    avaloniaObject.GetType(),
                    new PropertyMetadata());
                dependencyPropertys.Add(avaloniaProperty);
            }
            var resolvedValue = markup.ProvideValue(new ServiceProvider(avaloniaObject, avaloniaProperty));
            avaloniaObject.SetValue(avaloniaProperty, resolvedValue);
        }
    }
}
