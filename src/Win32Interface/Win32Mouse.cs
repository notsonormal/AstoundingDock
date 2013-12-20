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
    /// Virtual key Codes
    /// </summary>
    public enum VK : int
    {
        LBUTTON = 0x01,
        RBUTTON = 0x02,
        CANCEL  = 0x03,
        MBUTTON = 0x04,
    }

    [Flags]
    public enum MouseEventFlags : uint
    {
        LEFTDOWN    = 0x00000002,
        LEFTUP      = 0x00000004,
        MIDDLEDOWN  = 0x00000020,
        MIDDLEUP    = 0x00000040,
        MOVE        = 0x00000001,
        ABSOLUTE    = 0x00008000,
        RIGHTDOWN   = 0x00000008,
        RIGHTUP     = 0x00000010,
        WHEEL       = 0x00000800,
        XDOWN       = 0x00000080,
        XUP         = 0x00000100
    }

    public enum MouseEventDataXButtons : uint
    {
        XBUTTON1 = 0x00000001,
        XBUTTON2 = 0x00000002
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32Mouse
    {
        const int KEY_PRESSED = 0x8000;

        public static int[] GetMousePosition()
        {
            POINT point = new POINT();
            NativeMethods.GetCursorPos(ref point);
            return new int[] { point.X, point.Y };
        }

        public static bool IsLeftButtonPressed
        {
            get { return IsPressed(VK.LBUTTON); }
        }

        public static bool IsRightButtonPressed
        {
            get { return IsPressed(VK.RBUTTON); }
        }

        public static bool IsMiddleButtonPressed
        {
            get { return IsPressed(VK.MBUTTON); }
        }

        static bool IsPressed(VK key)
        {
            return Convert.ToBoolean(NativeMethods.GetAsyncKeyState((int)key) & KEY_PRESSED);
        }

        private static class NativeMethods
        {           
            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(ref POINT pt);
           
            [DllImport("user32.dll")]
            public static extern bool ScreenToClient(IntPtr hwnd, ref POINT pt);
           
            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(int virtualKeyCode);
        }
    }
}
