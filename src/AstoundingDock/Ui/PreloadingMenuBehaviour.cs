using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace AstoundingApplications.AstoundingDock.Ui
{
    public class PreloadingMenuBehaviour
    {
        #region EnabledPreloadingPropertyName Dependancy Property
        /// <summary>
        /// The EnabledPreloading attached property's name.
        /// </summary>
        public const string EnabledPreloadingPropertyName = "EnabledPreloading";

        /// <summary>
        /// Gets the value of the EnabledPreloading attached property 
        /// for a given dependency object.
        /// </summary>
        /// <param name="obj">The object for which the property value
        /// is read.</param>
        /// <returns>The value of the EnabledPreloading property of the specified object.</returns>
        public static bool GetEnabledPreloading(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledPreloadingProperty);
        }

        /// <summary>
        /// Sets the value of the EnabledPreloading attached property
        /// for a given dependency object. 
        /// </summary>
        /// <param name="obj">The object to which the property value
        /// is written.</param>
        /// <param name="value">Sets the EnabledPreloading value of the specified object.</param>
        public static void SetEnabledPreloading(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledPreloadingProperty, value);
        }

        /// <summary>
        /// Identifies the EnabledPreloading attached property.
        /// </summary>
        public static readonly DependencyProperty EnabledPreloadingProperty = DependencyProperty.RegisterAttached(
            EnabledPreloadingPropertyName,
            typeof(bool),
            typeof(PreloadingMenuBehaviour),
            new UIPropertyMetadata(false, OnEnabledPreloadingPropertyChanged));
        #endregion

        static void OnEnabledPreloadingPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            // NOTE: Weird, even though this isn't doing anything, having this attached property on each of the menu
            // items seems to stop the initial slow opening

            MenuItem control = depObj as MenuItem;
            if (control != null)
            {                
                if ((bool)e.NewValue)
                {
                    control.DataContextChanged += DataContextChanged;
                    control.UpdateLayout();
                }
                else
                {
                    control.DataContextChanged -= DataContextChanged;
                }
            }
        }

        static void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MenuItem control = sender as MenuItem;
            if (control != null)
            {
                control.UpdateLayout();
            }
        }
    }
}
