using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class FrameworkElementEx
    {
        /// <summary>
        /// Gets the DataContext of a WPF 'FrameworkElement', attempting to cast
        /// the DataContext to a particular type.
        /// </summary>
        public static T GetDataContext<T>(this FrameworkElement element) where T : class
        {
            return element == null ? null : element.DataContext as T;
        }
    }
}
