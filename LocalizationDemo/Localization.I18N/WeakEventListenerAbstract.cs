using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Localization.I18N
{
    abstract class WeakEventListenerAbstract : IWeakEventListener
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
                I18NManager.CultureChanged += OnCultureChanged;
            }

            protected override void StopListening(object source)
            {
                I18NManager.CultureChanged -= OnCultureChanged;
            }

            private void OnCultureChanged(object sender, EventArgs e)
            {
                DeliverEvent(null, e);
            }
        }

        public WeakEventListenerAbstract()
        {
            CultureChangedEventManager.AddListener(this);
        }
        ~WeakEventListenerAbstract()
        {
            CultureChangedEventManager.RemoveListener(this);
        }

        #region IWeakEventListener

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(CultureChangedEventManager))
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
