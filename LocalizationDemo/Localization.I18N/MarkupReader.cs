using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Markup;

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

        private static readonly ConditionalWeakTable<DependencyObject, List<DependencyProperty>> dependencyMap = new ConditionalWeakTable<DependencyObject, List<DependencyProperty>>();

        private readonly DependencyObject dependencyObject;
        private readonly DependencyProperty dependencyProperty;

        public object Value => dependencyObject.GetValue(dependencyProperty);

        public MarkupReader(DependencyObject dependencyObject, MarkupExtension markup)
        {
            this.dependencyObject = dependencyObject;
            if (!dependencyMap.TryGetValue(dependencyObject, out List<DependencyProperty> dependencyPropertys))
            {
                dependencyPropertys = new List<DependencyProperty>();
                dependencyMap.Add(dependencyObject, dependencyPropertys);
            }
            dependencyProperty = dependencyPropertys.FirstOrDefault(p => dependencyObject.ReadLocalValue(p) == DependencyProperty.UnsetValue);
            if (dependencyProperty == null)
            {
                dependencyProperty = DependencyProperty.RegisterAttached($"{dependencyObject.GetType().Name}{Guid.NewGuid()}" ,
                    typeof(object),
                    dependencyObject.GetType(),
                    new PropertyMetadata());
                dependencyPropertys.Add(dependencyProperty);
            }
            var resolvedValue = markup.ProvideValue(new ServiceProvider(dependencyObject, dependencyProperty));
            dependencyObject.SetValue(dependencyProperty, resolvedValue);
        }
    }
}
