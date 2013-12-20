using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace AstoundingApplications.Win32Interface
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32Image
    {
        public static bool DeleteObject(IntPtr handle)
        {
            return NativeMethods.DeleteObject(handle);
        }

        private static class NativeMethods
        {
            [DllImport("gdi32.dll", SetLastError = true)]
            public static extern bool DeleteObject(IntPtr hObject);
        }
    }
}
