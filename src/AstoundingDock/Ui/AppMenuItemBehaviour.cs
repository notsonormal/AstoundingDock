using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AstoundingDock.Messages;

namespace AstoundingApplications.AstoundingDock.Ui
{
    class AppMenuItemBehaviour
    {
        #region DoNotHideIfOpen Dependancy Property
        /// <summary>
        /// The DoNotHideIfOpen attached property's name.
        /// </summary>
        public const string DoNotHideIfOpenPropertyName = "DoNotHideIfOpen";

        /// <summary>
        /// Gets the value of the DoNotHideIfOpen attached property 
        /// for a given dependency object.
        /// </summary>
        /// <param name="obj">The object for which the property value
        /// is read.</param>
        /// <returns>The value of the DoNotHideIfOpen property of the specified object.</returns>
        public static bool GetDoNotHideIfOpen(DependencyObject obj)
        {
            return (bool)obj.GetValue(DoNotHideIfOpenProperty);
        }

        /// <summary>
        /// Sets the value of the DoNotHideIfOpen attached property
        /// for a given dependency object. 
        /// </summary>
        /// <param name="obj">The object to which the property value
        /// is written.</param>
        /// <param name="value">Sets the DoNotHideIfOpen value of the specified object.</param>
        public static void SetDoNotHideIfOpen(DependencyObject obj, bool value)
        {
            obj.SetValue(DoNotHideIfOpenProperty, value);
        }

        /// <summary>
        /// Identifies the DoNotHideIfOpen attached property.
        /// </summary>
        public static readonly DependencyProperty DoNotHideIfOpenProperty = DependencyProperty.RegisterAttached(
            DoNotHideIfOpenPropertyName,
            typeof(bool),
            typeof(AppMenuItemBehaviour),
            new UIPropertyMetadata(false, OnDoNotHideIfOpenPropertyChanged));
        #endregion

        static void OnDoNotHideIfOpenPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            MenuItem menuItem = depObj as MenuItem;
            if (menuItem != null)
            {
                if ((bool)e.NewValue)
                {
                    menuItem.SubmenuOpened += OnSubmenuOpened;
                    menuItem.SubmenuClosed += OnSubmenuClosed;
                }
                else
                {
                    menuItem.SubmenuOpened -= OnSubmenuOpened;
                    menuItem.SubmenuClosed -= OnSubmenuClosed;
                }
            }
        }

        static void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<MenuMessage>(MenuMessage.Opened());
        }

        static void OnSubmenuClosed(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<MenuMessage>(MenuMessage.Closed());
        }
    }
}
