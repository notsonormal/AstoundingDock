using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class IDataObjectEx
    {
        public static T GetData<T>(this IDataObject dragDataObject)
        {
            return (T)dragDataObject.GetData(typeof(T));
        }

        public static bool GetDataPresent<T>(this IDataObject dragDataObject)
        {
            return dragDataObject.GetDataPresent(typeof(T));
        }
    }
}
