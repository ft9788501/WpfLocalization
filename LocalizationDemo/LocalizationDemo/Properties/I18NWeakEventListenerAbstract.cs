using LocalizationDemo.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LocalizationDemo.Properties
{
   public abstract class I18NWeakEventListenerAbstract :  IWeakEventListener
    {
        public I18NWeakEventListenerAbstract()
        {
            I18NEventManager.AddListener(this);
        }
        ~I18NWeakEventListenerAbstract()
        {
            I18NEventManager.RemoveListener(this);
        }
        #region IWeakEventListener

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(I18NEventManager))
            {
                ReceiveWeakEvent();
                return true;
            }
            return false;
        }

        #endregion

        public abstract void ReceiveWeakEvent();
    }
}
