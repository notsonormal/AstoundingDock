using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;
using AstoundingApplications.AstoundingDock.Extensions;

namespace AstoundingApplications.AstoundingDock.Converters
{
    class IconToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Icon icon = value as Icon;
            if (icon != null)            
                return icon.ToImageSource();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
