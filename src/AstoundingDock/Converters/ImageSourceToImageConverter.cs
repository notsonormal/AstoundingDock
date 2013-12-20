using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Media.Imaging;
using AstoundingApplications.AstoundingDock.Utils;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;
using AstoundingApplications.AstoundingDock.Properties;
using System.IO;
using System.Drawing;
using AstoundingApplications.Win32Interface;
using System.Windows.Interop;
using AstoundingApplications.AstoundingDock.Extensions;

namespace AstoundingApplications.AstoundingDock.Converters
{
    class ImagePathToImageSourceConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GetImageSource(value as string, parameter as string);
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected ImageSource GetImageSource(string imagePath, string imageSize)
        {
            int desiredSize = ApplicationIcon.Medium;
            IconSize iconSize = IconSize.Medium;

            switch (imageSize)
            {
                case "Small":
                    iconSize = IconSize.Small;
                    desiredSize = ApplicationIcon.Small;
                    break;
                case "Medium":
                    iconSize = IconSize.Medium;
                    desiredSize = ApplicationIcon.Medium;
                    break;
                case "Large":
                    iconSize = IconSize.Large;
                    desiredSize = ApplicationIcon.Large;
                    break;
                case "Huge":
                    iconSize = IconSize.Large;
                    desiredSize = ApplicationIcon.Huge;
                    break;
            }

            var applicationIcon = new ApplicationIcon(imagePath);
            Bitmap bitmap = applicationIcon.GetImage(iconSize);
            if (bitmap == null)
                bitmap = Resources.UnknownAppIcon.ToBitmap();

            ImageSource source = bitmap.ToImageSource();

            if (source.Height > desiredSize || source.Width > desiredSize)
            {
                // Resize if too big.
                double scaleFactor = (double)desiredSize / Math.Max(source.Height, source.Width);
                int width = (int)Math.Round(scaleFactor * source.Width);
                int height = (int)Math.Round(scaleFactor * source.Height);

                return CreateResizedImage(source, width, height);
            }
            else if (source.Height != source.Width)
            {
                // Resize if not square.
                double scaleFactor = (double)Math.Min(source.Width, source.Height) / (double)desiredSize;
                int width = (int)(source.Width / scaleFactor);
                int height = (int)(source.Height / scaleFactor);

                return CreateResizedImage(source, width, height);
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Creates a new ImageSource with the specified width/height
        /// </summary>
        /// <remarks>
        /// http://blogs.msdn.com/b/delay/archive/2007/11/11/bigger-isn-t-always-better-how-to-resize-images-without-reloading-them-with-wpf.aspx
        /// </remarks>
        ImageSource CreateResizedImage(ImageSource source, int width, int height)
        {
            if (source == null)
                throw new ArgumentException("source");
            if (width <= 0)
                throw new ArgumentException("width");
            if (height <= 0)
                throw new ArgumentException("height");

            // Target Rect for the resize operation
            Rect rect = new Rect(0, 0, width, height);

            // Create a DrawingVisual/Context to render with
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(source, rect);
            }

            // Use RenderTargetBitmap to resize the original image
            RenderTargetBitmap resizedImage = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height,  // Resized dimensions
                96, 96,                             // Default DPI values
                PixelFormats.Default);              // Default pixel format

            resizedImage.Render(drawingVisual);
            resizedImage.Freeze();

            // Return the resized image
            return resizedImage;
        }
    }
}
