using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Localization.I18N
{
    abstract class CultureChangedReceiverAbstract
    {
        public CultureChangedReceiverAbstract()
        {
            I18NManager.CultureChanged += OnCultureChanged;
        }
        ~CultureChangedReceiverAbstract()
        {
            I18NManager.CultureChanged -= OnCultureChanged;
        }

        private void OnCultureChanged(object sender, CultureInfo e)
        {
            OnCultureChanged();
        }

        public abstract void OnCultureChanged();
    }
}
