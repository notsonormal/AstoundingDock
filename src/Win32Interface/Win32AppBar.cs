using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;

namespace AstoundingApplications.Win32Interface
{   
    /// <summary>
    /// The APPBARDATA Structure for use with the SHAppBarMessage
    /// </summary>
    /// <remarks>http://www.pinvoke.net/default.aspx/shell32/APPBARDATA.html</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public int cbSize;
        [SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible", 
            Justification="This is how the structure is defined in Win32")]
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public int lParam;
    }

    /// <summary>
    /// AppBar Edge
    /// </summary>
    public enum ABE : int
    {
        LEFT,
        TOP,
        RIGHT,
        BOTTOM
    }

    /// <summary>
    /// AppBar Message
    /// </summary>
    public enum ABM : uint
    {
        ABM_NEW = 0,
        ABM_REMOVE,
        ABM_QUERYPOS,
        ABM_SETPOS,
        ABM_GETSTATE,
        ABM_GETTASKBARPOS,
        ABM_ACTIVATE,
        ABM_GETAUTOHIDEBAR,
        ABM_SETAUTOHIDEBAR,
        ABM_WINDOWPOSCHANGED,
        ABM_SETSTATE
    }

    /// <summary>
    /// AppBar Notification
    /// </summary>
    public enum ABN : int
    {
        ABN_STATECHANGE = 0,
        ABN_POSCHANGED,
        ABN_FULLSCREENAPP,
        ABN_WINDOWARRANGE
    }

    /// <summary>
    /// AppBar Side
    /// </summary>
    [Flags]    
    public enum ABS : int
    {
        NEITHER     = 0x00000000,
        AUTOHIDE    = 0x00000001,
        ALWAYSONTOP = 0x00000002
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32AppBar
    {
        public static IntPtr SHAppBarMessage(ABM message, ref APPBARDATA data)
        {
            return NativeMethods.SHAppBarMessage((uint)message, ref data);
        }

        private static class NativeMethods
        {
            [DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);
        }
    }
}
