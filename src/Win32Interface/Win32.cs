using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace AstoundingApplications.Win32Interface
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    /// <summary>
    /// http://www.pinvoke.net/default.aspx/Structures/OSVERSIONINFO.html
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct OSVERSIONINFO
    {
        public uint dwOSVersionInfoSize;
        public uint dwMajorVersion;
        public uint dwMinorVersion;
        public uint dwBuildNumber;
        public uint dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
        public Int16 wServicePackMajor;
        public Int16 wServicePackMinor;
        public Int16 wSuiteMask;
        public Byte wProductType;
        public Byte wReserved;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32
    {
        /// <summary>1</summary>
        public static int TRUE { get { return 1; } }
        /// <summary>0</summary>
        public static int FALSE { get { return 0; } }       

        public static void ThrowWin32Exception()
        {
            Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        }

        public static uint GetCurrentThreadId()
        {
            return NativeMethods.GetCurrentThreadId();
        }

        public static bool AttachThreadInput(uint attachThis, uint attachToThis, bool attach)
        {
            return NativeMethods.AttachThreadInput(attachThis, attachToThis, attach);
        }

        public static IntPtr GetModuleHandle(string moduleName)
        {
            return NativeMethods.GetModuleHandle(moduleName);
        }

        internal static bool IsFunctionAvailable(string libraryName, string functionName)
        {
            IntPtr handle = NativeMethods.LoadLibrary(libraryName);
            UIntPtr result = NativeMethods.GetProcAddress(handle, functionName);
            return result != UIntPtr.Zero;
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern uint GetCurrentThreadId();

            [DllImport("user32.dll")]
            public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);
        }
    }
}
