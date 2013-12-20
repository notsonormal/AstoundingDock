using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class DependencyObjectEx
    {
        public static T FindVisualParent<T>(this DependencyObject element) where T : DependencyObject
        {
            DependencyObject parent = element;

            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        public static List<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            List<T> list = new List<T>();
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        list.Add((T)child);
                    }

                    List<T> childItems = FindVisualChildren<T>(child);
                    if (childItems != null && childItems.Count() > 0)
                    {
                        foreach (var item in childItems)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Returns the TemplatedParent property value if the object is a
        /// FrameworkElement or FrameworkContentElement.
        /// </summary>
        public static DependencyObject GetTemplatedParent(this DependencyObject depObj)
        {
            FrameworkElement fe = depObj as FrameworkElement;
            FrameworkContentElement fce = depObj as FrameworkContentElement;

            DependencyObject result;

            if (fe != null)
                result = fe.TemplatedParent;
            else if (fce != null)
                result = fce.TemplatedParent;
            else
                result = null;

            return result;
        }
    }
}
