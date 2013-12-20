using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AstoundingApplications.AstoundingDock.Utils;
using AstoundingApplications.Win32Interface;
using System.Diagnostics.Contracts;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class IconEx
    {
        /// <summary>
        /// Convert Icon object to the WPF 'ImageSource' object.
        /// Usings the BitmapEx extension class.
        /// </summary>
        public static ImageSource ToImageSource(this Icon icon)
        {
            ImageSource source = icon.ToBitmap().ToImageSource();
            icon.Dispose();
            icon = null;

            return source;
        }
    }
}
