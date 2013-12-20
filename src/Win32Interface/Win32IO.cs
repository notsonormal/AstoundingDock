using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.IO;

namespace AstoundingApplications.Win32Interface
{
    // The CharSet must match the CharSet of the corresponding PInvoke signature
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WIN32_FIND_DATA
    {
        public FileAttributes dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;        
    }    

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public static class Win32IO
    {        
        // http://www.pinvoke.net/default.aspx/kernel32.findfirstfile
        public static SafeFindHandle FindFirstFile(string path, out WIN32_FIND_DATA findData)
        {
            return NativeMethods.FindFirstFile(path, out findData);
        }

        public static bool FindNextFile(SafeFindHandle findHandle, out WIN32_FIND_DATA findData)
        {
            return NativeMethods.FindNextFile(findHandle, out findData);
        }

        // http://www.pinvoke.net/default.aspx/kernel32.findfirstfile
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            internal SafeFindHandle()
                : base(true)
            {
            }

            public SafeFindHandle(IntPtr preExistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                base.SetHandle(preExistingHandle);
            }

            protected override bool ReleaseHandle()
            {
                if (!(IsInvalid || IsClosed))
                {
                    return NativeMethods.FindClose(this);
                }
                return (IsInvalid || IsClosed);
            }

            protected override void Dispose(bool disposing)
            {
                if (!(IsInvalid || IsClosed))
                {
                    NativeMethods.FindClose(this);
                }
                base.Dispose(disposing);
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern SafeFindHandle FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool FindClose(SafeHandle hFindFile);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool FindNextFile(SafeHandle hFindFile, out WIN32_FIND_DATA lpFindFileData);
        }
    }
}
