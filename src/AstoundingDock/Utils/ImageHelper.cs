using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AstoundingApplications.AstoundingDock.Utils
{
    static class ImageHelper
    {
        /// <summary>
        /// Releases bitmap from memory. Resolves memory leak.
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/871610/bitmap-save-huge-memory-leak
        /// </remarks>
        public static void ReleaseBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return;

            // Need to explictly call the garbage collecter after disposing.
            bitmap.Dispose();
            bitmap = null;            

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }
        /// <summary>
        /// Releases image from memory. Resolves memory leak.
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/871610/bitmap-save-huge-memory-leak
        /// </remarks>
        public static void ReleaseImage(Image image)
        {
            if (image == null)
                return;            

            // Need to explictly call the garbage collecter after disposing.
            image.Dispose();
            image = null;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
        }
    }
}
