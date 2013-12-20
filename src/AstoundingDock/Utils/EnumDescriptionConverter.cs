using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace AstoundingApplications.AstoundingDock.Utils
{
    // http://blog.spencen.com/2009/04/19/binding-an-enum-property-to-a-combobox-using-customized-text.aspx
    class EnumDescriptionConverter : EnumConverter
    {
        public EnumDescriptionConverter(Type enumType) : base(enumType) { }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                var enumType = value.GetType();
                if (enumType.IsEnum)
                    return GetDisplayName(value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        private string GetDisplayName(object enumValue)
        {
            var displayNameAttribute = EnumType.GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;

            if (displayNameAttribute != null)
                return displayNameAttribute.Description;

            return Enum.GetName(EnumType, enumValue);
        }
    }
}
