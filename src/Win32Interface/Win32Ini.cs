using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace AstoundingApplications.Win32Interface
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32Ini
    {        
        public static void IniWriteValue(string path, string Section, string Key, string Value)
        {
            NativeMethods.WritePrivateProfileString(Section, Key, Value, path);
        }

        public static string IniReadValue(string path, string section, string key)
        {
            StringBuilder buffer = new StringBuilder(255);
            int i = NativeMethods.GetPrivateProfileString(section, key, "", buffer, 255, path);
            return buffer.ToString();
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern int WritePrivateProfileString(string section, string key, 
                string val, string filePath);

            [DllImport("kernel32.dll")]
            public static extern int GetPrivateProfileString(string section, string key, 
                string def, StringBuilder retVal, int size, string filePath);
        }
    }
}
