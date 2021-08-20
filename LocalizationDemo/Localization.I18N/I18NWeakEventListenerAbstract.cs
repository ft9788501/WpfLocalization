using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Localization.I18N
{
    abstract class I18NWeakEventListenerAbstract : IWeakEventListener
    {
        class I18NEventManager : WeakEventManager
        {
            private static I18NEventManager CurrentManager
            {
                get
                {
                    I18NEventManager manager = (I18NEventManager)GetCurrentManager(typeof(I18NEventManager));
                    if (manager == null)
                    {
                        manager = new I18NEventManager();
                        SetCurrentManager(typeof(I18NEventManager), manager);
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
