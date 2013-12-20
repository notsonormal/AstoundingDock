using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AstoundingApplications.AstoundingDock.Utils;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Diagnostics.Contracts;

namespace AstoundingApplications.AstoundingDock.Converters
{
    class ImagePathToImageConverter : ImagePathToImageSourceConverter
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Image() { Source = GetImageSource(value as string, parameter as string) };
        }
    }
}
