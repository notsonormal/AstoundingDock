using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using AstoundingApplications.AstoundingDock.Extensions;

namespace AstoundingApplications.AstoundingDock.Utils
{
    class WpfHelper
    {
        public static readonly List<string> AvailableThemes = new List<string>
        {
            "Ballistic",
            "Developer",
            "Gemini",
            "GlassBlue",
            "OdysseyBlue",
        };

        public static void LoadTheme(string themeName)
        {
            string assembyName = Helper.SplitCamelCase(themeName)[0];

            // e.g. /Odyssey;component/OdysseyBlue.xaml
            Uri uri = new Uri(String.Format("/{0};component/{1}.xaml", assembyName, themeName), UriKind.Relative);
            Application.Current.Resources.Source = uri;

            //try
            //{
                //Uri uri = new Uri(String.Format("Skins/{0}.xaml", themeName), UriKind.Relative);
                //Application.Current.Resources.Source = uri;
            //}
            //catch
            //{
                //Uri uri = new Uri(String.Format("../Skins/{0}.xaml", themeName), UriKind.Relative);
                //Application.Current.Resources.Source = uri;
            //}
        }

        public static void TransformToPixels(Visual visual, double unitX, double unitY, out int pixelX, out int pixelY)
        {
            Matrix matrix;
            var source = PresentationSource.FromVisual(visual);
            if (source != null)
            {
                matrix = source.CompositionTarget.TransformToDevice;
            }
            else
            {
                using (var src = new HwndSource(new HwndSourceParameters()))
                {
                    matrix = src.CompositionTarget.TransformToDevice;
                }
            }

            pixelX = (int)(matrix.M11 * unitX);
            pixelY = (int)(matrix.M22 * unitY);
        }

        /// <summary>
        /// Writes information about 'elem' to the console.
        /// </summary>
        public static void DumpElement(object elem, bool isInitialElement, int indentLevel)
        {
            string indentation = new string(' ', indentLevel);
            string typeName = elem == null ? "(null)" : elem.GetType().Name;
            string text = String.Format("{0}{1}) {2}", indentation, indentLevel, typeName);

            if (isInitialElement)
                text += " [YOU CLICKED HERE]";

            Console.WriteLine(text);
        }        

        /// <summary>
        /// Retrieves an array of dependancy objects underneath the mouse.
        /// </summary>
        /// <remarks>http://prabu-guru.blogspot.com/2010/05/how-to-get-dependencyobject-under-mouse.html</remarks>
        public class HitTest
        {
            public List<DependencyObject> Found { get; private set; }

            public HitTest(DependencyObject element, Point point)
            {
                Found = new List<DependencyObject>();

                VisualTreeHelper.HitTest(
                    element as Visual, null,
                    CollectAllVisuals_Callback,
                    new PointHitTestParameters(point));
            }

            HitTestResultBehavior CollectAllVisuals_Callback(HitTestResult result)
            {
                if (result == null || result.VisualHit == null)
                    return HitTestResultBehavior.Stop;

                Found.Add(result.VisualHit);
                return HitTestResultBehavior.Continue;
            }
        }

        public static class VisualTreeDumper
        {
            public static void Dump(DependencyObject originalElement)
            {
                DependencyObject closestVisualAncestor = FindClosestVisualAncestor(originalElement);
                DependencyObject visualRoot = FindVisualTreeRoot(originalElement);

                Console.WriteLine("DUMPING VISUAL TREE:");
                Console.WriteLine("Original Element: " + originalElement.GetType().Name);

                string closestParentText = closestVisualAncestor == originalElement ? "(self)" : closestVisualAncestor.GetType().Name;
                Console.WriteLine("Closest Visual Ancestor to Original Element: " + closestParentText);

                DependencyObject templatedParent = closestVisualAncestor.GetTemplatedParent();
                string templatedParentType = templatedParent == null ? "(null)" : templatedParent.GetType().Name;
                Console.WriteLine("TemplatedParent of Closest Visual Ancestor: " + templatedParentType);

                Console.WriteLine();

                // Write out the visual tree to the console.
                DoDump(visualRoot, closestVisualAncestor, 0);

                Console.WriteLine("************************************************************\n");
            }

            /// <summary>
            /// This method is necessary in case the user clicks on a ContentElement,
            /// which is not part of the visual tree.  It will walk up the logical
            /// tree, if necessary, to find the first ancestor in the visual tree.
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            static DependencyObject FindClosestVisualAncestor(DependencyObject initial)
            {
                if (initial is Visual || initial is Visual3D)
                    return initial;

                DependencyObject current = initial;
                DependencyObject result = initial;

                while (current != null)
                {
                    result = current;
                    if (current is Visual || current is Visual3D)
                    {
                        result = current;
                        break;
                    }
                    else
                    {
                        // If we're in Logical Land then we must walk up the logical tree
                        // until we find a Visual/Visual3D to get us back to Visual Land.
                        current = LogicalTreeHelper.GetParent(current);
                    }
                }

                return result;
            }

            static DependencyObject FindVisualTreeRoot(DependencyObject initial)
            {
                DependencyObject current = initial;
                DependencyObject result = initial;

                while (current != null)
                {
                    result = current;
                    if (current is Visual || current is Visual3D)
                    {
                        current = VisualTreeHelper.GetParent(current);
                    }
                    else
                    {
                        // If we're in Logical Land then we must walk up the logical tree
                        // until we find a Visual/Visual3D to get us back to Visual Land.
                        current = LogicalTreeHelper.GetParent(current);
                    }
                }

                return result;
            }

            static void DoDump(DependencyObject current, DependencyObject initial, int indentLevel)
            {
                DumpElement(current, current == initial, indentLevel);

                int visualChildrenCount = VisualTreeHelper.GetChildrenCount(current);

                for (int i = 0; i < visualChildrenCount; ++i)
                {
                    DependencyObject visualChild = VisualTreeHelper.GetChild(current, i);
                    DoDump(visualChild, initial, indentLevel + 1);
                }
            }
        }

        public static class LogicalTreeDumper
        {
            public static void Dump(DependencyObject originalElement)
            {
                DependencyObject closestLogicalAncestor = FindClosestLogicalAncestor(originalElement);
                DependencyObject logicalRoot = FindLogicalTreeRoot(closestLogicalAncestor);

                Console.WriteLine("DUMPING LOGICAL TREE:");
                Console.WriteLine("Original Element: " + originalElement.GetType().Name);

                string closestParentText = closestLogicalAncestor == originalElement ? "(self)" : closestLogicalAncestor.GetType().Name;
                Console.WriteLine("Closest Logical Ancestor to Original Element: " + closestParentText);

                DependencyObject templatedParent = closestLogicalAncestor.GetTemplatedParent();
                string templatedParentType = templatedParent == null ? "(null)" : templatedParent.GetType().Name;
                Console.WriteLine("TemplatedParent of Closest Logical Ancestor: " + templatedParentType);

                Console.WriteLine();

                // Write out the logical tree to the console.
                DoDump(logicalRoot, closestLogicalAncestor, 0);

                Console.WriteLine("************************************************************\n");
            }

            /// <summary>
            /// This method is necessary in case the user clicks on an element
            /// which is not part of a logical tree.  It finds the closest ancestor
            /// element which is in a logical tree.
            /// </summary>
            /// <param name="initial">The element on which the user clicked.</param>
            static DependencyObject FindClosestLogicalAncestor(DependencyObject initial)
            {
                DependencyObject current = initial;
                DependencyObject result = initial;

                while (current != null)
                {
                    DependencyObject logicalParent = LogicalTreeHelper.GetParent(current);
                    if (logicalParent != null)
                    {
                        result = current;
                        break;
                    }
                    current = VisualTreeHelper.GetParent(current);
                }

                return result;
            }

            /// <summary>
            /// Walks up the logical tree starting at 'initial' and returns
            /// the topmost element in that tree.
            /// </summary>
            /// <param name="initial">It is assumed that this element is in a logical tree.</param>
            static DependencyObject FindLogicalTreeRoot(DependencyObject initial)
            {
                DependencyObject current = initial;
                DependencyObject result = initial;

                while (current != null)
                {
                    result = current;
                    current = LogicalTreeHelper.GetParent(current);
                }

                return result;
            }

            static void DoDump(object current, object initial, int indentLevel)
            {
                DumpElement(current, current == initial, indentLevel);

                // The logical tree can contain any type of object, not just 
                // instances of DependencyObject subclasses.  LogicalTreeHelper
                // only works with DependencyObject subclasses, so we must be
                // sure that we do not pass it an object of the wrong type.
                DependencyObject depObj = current as DependencyObject;

                if (depObj != null)
                    foreach (object logicalChild in LogicalTreeHelper.GetChildren(depObj))
                        DoDump(logicalChild, initial, indentLevel + 1);
            }
        }
    }
}
