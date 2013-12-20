using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class GenericEx
    {
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) 
                throw new ArgumentNullException("source");
            return list.Contains(source);
        }

        public static U IfNotNull<T, U>(this T t, Func<T, U> fn)
        {
            return t != null ? fn(t) : default(U);
        }
    }
}
