using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Drawing;
using AstoundingApplications.Win32Interface;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class BitmapEx
    {
        /// <summary>
        /// Converts a Bitmap object to WPF 'ImageSource' object and 
        /// garbage collects to Bitmap object.
        /// </summary>
        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                ImageSource source = Imaging.CreateBitmapSourceFromHBitmap(
                        hBitmap, 
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                );

                source.Freeze();
                return source;
            }
            finally
            {
                if (!Win32Image.DeleteObject(hBitmap))
                    Debug.WriteLine("BitmapEx.ToImageSource() - Unable to Delete hBitmap");   
            
                bitmap.Dispose();
                bitmap = null;
                GC.Collect();
            }
        }
    }
}
