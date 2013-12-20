using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.ComponentModel;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class EnumEx
    {
        public static T Parse<T>(object value)
        {
            return (T)Enum.Parse(typeof(T), value.ToString(), true);
        }

        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])
                fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static IDictionary<string, int> ToDictionary(this Enum enumeration)
        {
            int value = (int)(object)enumeration;
            return Enum.GetValues(enumeration.GetType()).OfType<int>().
                Where(v => (v & value) == value).
                ToDictionary(v => Enum.GetName(enumeration.GetType(), v));
        }
    }
}
