using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.Utils;
using System.Globalization;
using System.Windows.Data;
using System.Diagnostics.Contracts;

namespace AstoundingApplications.AstoundingDock.Converters
{
    class IconSizeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            if (value != null && value is IconSize)
            {
                IconSize imageSize = (IconSize)value;
                switch (imageSize)
                {
                    case IconSize.Large:
                        return ApplicationIcon.Large;
                    case IconSize.Medium:
                        return ApplicationIcon.Medium;
                    case IconSize.Small:
                        return ApplicationIcon.Small;
                }
            }
            return ApplicationIcon.Medium;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
    }
}
