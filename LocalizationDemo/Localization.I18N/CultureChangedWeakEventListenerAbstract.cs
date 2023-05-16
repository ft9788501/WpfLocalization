using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;

namespace Localization.I18N
{
    abstract class CultureChangedWeakEventListenerAbstract
    {
        public CultureChangedWeakEventListenerAbstract()
        {
            I18NManager.CultureChanged += OnCultureChanged;
        }
        ~CultureChangedWeakEventListenerAbstract()
        {
            I18NManager.CultureChanged -= OnCultureChanged;
        }

        private void OnCultureChanged(object sender, CultureInfo e)
        {
            ReceiveWeakEvent();
        }

        public abstract void ReceiveWeakEvent();
    }
}
