using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    // Uses IConvertibleEx
    public static class XElementEx
    {
        public static T GetValue<T>(this XElement element, string name, T defaultValue)
        {
            try
            {
                return GetValue<T>(element, name);
            }
            catch (InvalidCastException ex)
            {
                Debug.Print("GetValue<{0}> failed for element {1}, name {2}. Exception: {3}",
                    typeof(T), element, name, ex);
                return defaultValue;
            }
        }

        public static T GetValue<T>(this XElement element, string name)
        {
            return GetValue(element, name).To<T>();
        }

        public static string GetValue(this XElement element, string name)
        {
            XElement subElement = element.Element(name);
            if (subElement == null || String.IsNullOrWhiteSpace(subElement.Value))
                return null;
            return subElement.Value;
        }

        public static T GetAttribute<T>(this XElement element, string name, T defaultValue)
        {
            try
            {
                return GetAttribute<T>(element, name);
            }
            catch (InvalidCastException ex)
            {
                Debug.Print("GetAttribute<{0}> failed for element {1}, name {2}. Exception: {3}",
                    typeof(T), element, name, ex);
                return defaultValue;
            }
        }

        public static T GetAttribute<T>(this XElement element, string name)
        {
            return GetAttribute(element, name).To<T>();
        }

        public static string GetAttribute(this XElement element, string name)
        {
            XAttribute attribute = element.Attribute(name);
            if (attribute == null || String.IsNullOrWhiteSpace(attribute.Value))
                return null;
            return attribute.Value;
        }
    }
}
