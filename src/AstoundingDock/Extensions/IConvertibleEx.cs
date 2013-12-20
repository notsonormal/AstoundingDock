using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class IConvertibleEx
    {        
        public static T To<T>(this IConvertible obj, T defaultValue)
        {
            try
            {
                return To<T>(obj);
            }
            catch (FormatException ex)
            {
                Debug.Print("To<{0}> failed for value {1}, exception {2}",
                    typeof(T), obj, ex);
                return defaultValue;
            }
            catch (ArgumentNullException ex)
            {
                Debug.Print("To<{0}> failed for value {1}, exception {2}",
                    typeof(T), obj, ex);
                return defaultValue;
            }
            catch (InvalidCastException ex)
            {
                Debug.Print("To<{0}> failed for value {1}, exception {2}", 
                    typeof(T), obj, ex);
                return defaultValue;
            }
        }

        public static T To<T>(this IConvertible obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}
