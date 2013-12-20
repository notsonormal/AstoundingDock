using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics.Contracts;

namespace AstoundingApplications.AstoundingDock.Extensions
{
    public static class PathEx
    {
        /// <summary>
        /// Returns true if the path ends with the extension. 
        /// </summary>
        public static bool HasExtension(string path, string extension) 
        {
            return HasExtension(path, extension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns true if the path ends with the extension. 
        /// </summary>
        public static bool HasExtension(string path, string extension, StringComparison comparision)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(extension))
                return false;

            return String.Equals(Path.GetExtension(path), extension, comparision);
        }
    }
}
