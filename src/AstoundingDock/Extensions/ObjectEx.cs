using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class ObjectEx
    {
        public static T As<T>(this object value)
        {
            return (value != null && value is T) ? (T)value : default(T);
        }

        /// <summary>
        /// Get all properties and their values of an object.
        /// </summary>
        public static Dictionary<string, object> GetProperties(this object source)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            if (source != null)
            {
                foreach (var property in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    values[property.Name] = property.GetValue(source, null);
                }
            }
            return values;
        }
    }
}
