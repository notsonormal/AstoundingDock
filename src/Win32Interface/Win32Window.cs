using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Diagnostics;

namespace AstoundingApplications.Win32Interface
{

    /// <summary>
    /// System Info Parameters
    /// </summary>
    [Flags]
    public enum SPI
    {
        SPI_GETBEEP = 1,
        SPI_SETBEEP = 2,
        SPI_GETMOUSE = 3,
        SPI_SETMOUSE = 4,
        SPI_GETBORDER = 5,
        SPI_SETBORDER = 6,
        SPI_GETKEYBOARDSPEED = 10,
        SPI_SETKEYBOARDSPEED = 11,
        SPI_LANGDRIVER = 12,
        SPI_ICONHORIZONTALSPACING = 13,
        SPI_GETSCREENSAVETIMEOUT = 14,
        SPI_SETSCREENSAVETIMEOUT = 15,
        SPI_GETSCREENSAVEACTIVE = 16,
        SPI_SETSCREENSAVEACTIVE = 17,
        SPI_GETGRIDGRANULARITY = 18,
        SPI_SETGRIDGRANULARITY = 19,
        SPI_SETDESKWALLPAPER = 20,
        SPI_SETDESKPATTERN = 21,
        SPI_GETKEYBOARDDELAY = 22,
        SPI_SETKEYBOARDDELAY = 23,
        SPI_ICONVERTICALSPACING = 24,
        SPI_GETICONTITLEWRAP = 25,
        SPI_SETICONTITLEWRAP = 26,
        SPI_GETMENUDROPALIGNMENT = 27,
        SPI_SETMENUDROPALIGNMENT = 28,
        SPI_SETDOUBLECLKWIDTH = 29,
        SPI_SETDOUBLECLKHEIGHT = 30,
        SPI_GETICONTITLELOGFONT = 31,
        SPI_SETDOUBLECLICKTIME = 32,
        SPI_SETMOUSEBUTTONSWAP = 33,
        SPI_SETICONTITLELOGFONT = 34,
        SPI_GETFASTTASKSWITCH = 35,
        SPI_SETFASTTASKSWITCH = 36,
        SPI_SETDRAGFULLWINDOWS = 37,
        SPI_GETDRAGFULLWINDOWS = 38,
        SPI_GETNONCLIENTMETRICS = 41,
        SPI_SETNONCLIENTMETRICS = 42,
        SPI_GETMINIMIZEDMETRICS = 43,
        SPI_SETMINIMIZEDMETRICS = 44,
        SPI_GETICONMETRICS = 45,
        SPI_SETICONMETRICS = 46,
        SPI_SETWORKAREA = 47,
        SPI_GETWORKAREA = 48,
        SPI_SETPENWINDOWS = 49,
        SPI_GETHIGHCONTRAST = 66,
        SPI_SETHIGHCONTRAST = 67,
        SPI_GETKEYBOARDPREF = 68,
        SPI_SETKEYBOARDPREF = 69,
        SPI_GETSCREENREADER = 70,
        SPI_SETSCREENREADER = 71,
        SPI_GETANIMATION = 72,
        SPI_SETANIMATION = 73,
        SPI_GETFONTSMOOTHING = 74,
        SPI_SETFONTSMOOTHING = 75,
        SPI_SETDRAGWIDTH = 76,
        SPI_SETDRAGHEIGHT = 77,
        SPI_SETHANDHELD = 78,
        SPI_GETLOWPOWERTIMEOUT = 79,
        SPI_GETPOWEROFFTIMEOUT = 80,
        SPI_SETLOWPOWERTIMEOUT = 81,
        SPI_SETPOWEROFFTIMEOUT = 82,
        SPI_GETLOWPOWERACTIVE = 83,
        SPI_GETPOWEROFFACTIVE = 84,
        SPI_SETLOWPOWERACTIVE = 85,
        SPI_SETPOWEROFFACTIVE = 86,
        SPI_SETCURSORS = 87,
        SPI_SETICONS = 88,
        SPI_GETDEFAULTINPUTLANG = 89,
        SPI_SETDEFAULTINPUTLANG = 90,
        SPI_SETLANGTOGGLE = 91,
        SPI_GETWINDOWSEXTENSION = 92,
        SPI_SETMOUSETRAILS = 93,
        SPI_GETMOUSETRAILS = 94,
        SPI_SETSCREENSAVERRUNNING = 97,
        SPI_SCREENSAVERRUNNING = SPI_SETSCREENSAVERRUNNING,
        SPI_GETFILTERKEYS = 50,
        SPI_SETFILTERKEYS = 51,
        SPI_GETTOGGLEKEYS = 52,
        SPI_SETTOGGLEKEYS = 53,
        SPI_GETMOUSEKEYS = 54,
        SPI_SETMOUSEKEYS = 55,
        SPI_GETSHOWSOUNDS = 56,
        SPI_SETSHOWSOUNDS = 57,
        SPI_GETSTICKYKEYS = 58,
        SPI_SETSTICKYKEYS = 59,
        SPI_GETACCESSTIMEOUT = 60,
        SPI_SETACCESSTIMEOUT = 61,
        SPI_GETSERIALKEYS = 62,
        SPI_SETSERIALKEYS = 63,
        SPI_GETSOUNDSENTRY = 64,
        SPI_SETSOUNDSENTRY = 65,
        SPI_GETSNAPTODEFBUTTON = 95,
        SPI_SETSNAPTODEFBUTTON = 96,
        SPI_GETMOUSEHOVERWIDTH = 98,
        SPI_SETMOUSEHOVERWIDTH = 99,
        SPI_GETMOUSEHOVERHEIGHT = 100,
        SPI_SETMOUSEHOVERHEIGHT = 101,
        SPI_GETMOUSEHOVERTIME = 102,
        SPI_SETMOUSEHOVERTIME = 103,
        SPI_GETWHEELSCROLLLINES = 104,
        SPI_SETWHEELSCROLLLINES = 105,
        SPI_GETMENUSHOWDELAY = 106,
        SPI_SETMENUSHOWDELAY = 107,
        SPI_GETSHOWIMEUI = 110,
        SPI_SETSHOWIMEUI = 111,
        SPI_GETMOUSESPEED = 112,
        SPI_SETMOUSESPEED = 113,
        SPI_GETSCREENSAVERRUNNING = 114,
        SPI_GETDESKWALLPAPER = 115,
        //SPI_GETACTIVEWINDOWTRACKING = &H1000,
        //SPI_SETACTIVEWINDOWTRACKING = &H1001,
        //SPI_GETMENUANIMATION = &H1002,
        //SPI_SETMENUANIMATION = &H1003,
        //SPI_GETCOMBOBOXANIMATION = &H1004,
        //SPI_SETCOMBOBOXANIMATION = &H1005,
        //SPI_GETLISTBOXSMOOTHSCROLLING = &H1006,
        //SPI_SETLISTBOXSMOOTHSCROLLING = &H1007,
        //SPI_GETGRADIENTCAPTIONS = &H1008,
        //SPI_SETGRADIENTCAPTIONS = &H1009,
        //SPI_GETKEYBOARDCUES = &H100A,
        //SPI_SETKEYBOARDCUES = &H100B,
        //SPI_GETMENUUNDERLINES = SPI_GETKEYBOARDCUES,
        //SPI_SETMENUUNDERLINES = SPI_SETKEYBOARDCUES,
        //SPI_GETACTIVEWNDTRKZORDER = &H100C,
        //SPI_SETACTIVEWNDTRKZORDER = &H100D,
        //SPI_GETHOTTRACKING = &H100E,
        //SPI_SETHOTTRACKING = &H100F,
        //SPI_GETMENUFADE = &H1012,
        //SPI_SETMENUFADE = &H1013,
        //SPI_GETSELECTIONFADE = &H1014,
        //SPI_SETSELECTIONFADE = &H1015,
        //SPI_GETTOOLTIPANIMATION = &H1016,
        //SPI_SETTOOLTIPANIMATION = &H1017,
        //SPI_GETTOOLTIPFADE = &H1018,
        //SPI_SETTOOLTIPFADE = &H1019,
        //SPI_GETCURSORSHADOW = &H101A,
        //SPI_SETCURSORSHADOW = &H101B,
        //SPI_GETUIEFFECTS = &H103E,
        //SPI_SETUIEFFECTS = &H103F,
        //SPI_GETFOREGROUNDLOCKTIMEOUT = &H2000,
        //SPI_SETFOREGROUNDLOCKTIMEOUT = &H2001,
        //SPI_GETACTIVEWNDTRKTIMEOUT = &H2002,
        //SPI_SETACTIVEWNDTRKTIMEOUT = &H2003,
        //SPI_GETFOREGROUNDFLASHCOUNT = &H2004,
        //SPI_SETFOREGROUNDFLASHCOUNT = &H2005,
        //SPI_GETCARETWIDTH = &H2006,
        //SPI_SETCARETWIDTH = &H2007
    }

    /// <summary>
    /// User notification state
    /// </summary>
    /// <remarks>http://www.pinvoke.net/default.aspx/shell32.shqueryusernotificationstate</remarks>
    [Flags]
    public enum QUERY_USER_NOTIFICATION_STATE
    {
        QUNS_NOT_PRESENT = 1,
        QUNS_BUSY = 2,
        QUNS_RUNNING_D3D_FULL_SCREEN = 3,
        QUNS_PRESENTATION_MODE = 4,
        QUNS_ACCEPTS_NOTIFICATIONS = 5,
        QUNS_QUIET_TIME = 6 
    };

    /// <summary>
    /// Window State
    /// </summary>
    /// <remarks>http://www.pinvoke.net/default.aspx/user32/ShowState.html</remarks>
    public enum SW : int
    {
        HIDE = 0,
        SHOWNORMAL = 1,
        SHOWMAXIMIZED = 2,
        MAXIMIZE = 3,
        SHOWNOACTIVATE = 4,
        SHOW = 5,
        MINIMIZE = 6,
        SHOWMINNOACTIVE = 7,
        SHOWNA = 8,
        RESTORE = 9,
        SHOWDEFAULT = 10,
        FORCEMINIZED = 11,
    }

    /// <summary>
    /// Window sizing/positining flags
    /// </summary>
    [Flags()]
    public enum SWP : uint
    {
        ASYNCWINDOWPOS = 0x4000,
        DEFERERASE = 0x2000,
        DRAWFRAME = 0x0020,
        FRAMECHANGED = 0x0020,
        HIDEWINDOW = 0x0080,
        NOACTIVATE = 0x0010,
        NOCOPYBITS = 0x0100,
        NOMOVE = 0x0002,
        NOOWNERZORDER = 0x0200,
        NOREDRAW = 0x0008,
        NOREPOSITION = 0x0200,
        NOSENDCHANGING = 0x0400,
        NOSIZE = 0x0001,
        NOZORDER = 0x0004,
        SHOWWINDOW = 0x0040,
    }

    /// <summary>
    /// GetWindowLong Options
    /// </summary>
    public enum GWL : int
    {
        /// <summary> Get/set window procedure address. </summary>
        WNDPROC = (-4),
        /// <summary> Get/set application instance handle. </summary>        
        HINSTANCE = (-6),
        /// <summary> Get/set child window identifier. </summary>
        ID = (-12),
        /// <summary> Get/set window style. </summary>
        STYLE = (-16),
        /// <summary> Get/set extended window style. </summary>
        EXSTYLE = (-20),
        /// <summary> Get/set user pData associated with the window. </summary>
        USERDATA = (-21)
    }

    /// <summary>
    /// Window Styles
    /// </summary>
    [Flags]
    public enum WS : int
    {
        OVERLAPPED = 0,
        //POPUP = 0x80000000,
        CHILD = 0x40000000,
        MINIMIZE = 0x20000000,
        VISIBLE = 0x10000000,
        DISABLED = 0x8000000,
        CLIPSIBLINGS = 0x4000000,
        CLIPCHILDREN = 0x2000000,
        MAXIMIZE = 0x100000,
        CAPTION = 0xC00000,      // BORDER or DLGFRAME  
        BORDER = 0x800000,
        DLGFRAME = 0x400000,
        VSCROLL = 0x200000,
        HSCROLL = 0x100000,
        SYSMENU = 0x80000,
        THICKFRAME = 0x40000,
        GROUP = 0x20000,
        TABSTOP = 0x10000,
        MINIMIZEBOX = 0x20000,
        MAXIMIZEBOX = 0x10000,
        TILED = OVERLAPPED,
        ICONIC = MINIMIZE,
        SIZEBOX = THICKFRAME
    }

    /// <summary>
    /// Extended Window Styles
    /// </summary>
    [Flags]
    public enum WS_EX : int
    {
        DLGMODALFRAME = 0x0001,
        NOPARENTNOTIFY = 0x0004,
        TOPMOST = 0x0008,
        ACCEPTFILES = 0x0010,
        TRANSPARENT = 0x0020,
        MDICHILD = 0x0040,
        TOOLWINDOW = 0x80,
        WINDOWEDGE = 0x0100,
        CLIENTEDGE = 0x0200,
        CONTEXTHELP = 0x0400,
        RIGHT = 0x1000,
        LEFT = 0x0000,
        RTLREADING = 0x2000,
        LTRREADING = 0x0000,
        LEFTSCROLLBAR = 0x4000,
        RIGHTSCROLLBAR = 0x0000,
        CONTROLPARENT = 0x10000,
        STATICEDGE = 0x20000,
        APPWINDOW = 0x40000,
        OVERLAPPEDWINDOW = (WINDOWEDGE | CLIENTEDGE),
        PALETTEWINDOW = (WINDOWEDGE | TOOLWINDOW | TOPMOST),
        LAYERED = 0x00080000,
        NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
        LAYOUTRTL = 0x00400000, // Right to left mirroring
        COMPOSITED = 0x02000000,
        NOACTIVATE = 0x08000000
    }    

    /// <summary>
    /// A handle to the window to precede the positioned window in the Z order.
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/es-ar/library/ms632681</remarks>
    public enum HWND : int
    {      
        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        TOPMOST = -1,
        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        NOTOPMOST = -2,
        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect 
        /// if the window is already a non-topmost window.
        /// </summary>
        TOP = 0,
        /// <summary>
        /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window 
        /// loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>        
        BOTTOM = 1    
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWINFO
    {
        public uint cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        public WINDOWINFO(Boolean? filler)
            : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
        }
    }

    public enum HOOKTYPE : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    public delegate int HOOKPROC(int code, IntPtr wParam, IntPtr lParam);

    // http://msdn.microsoft.com/en-us/library/ms644977(v=VS.85).aspx
    public enum HCBT : int
    {
        MOVESIZE = 0,
        MINMAX = 1,
        QS = 2,
        CREATEWND = 3,
        DESTROYWND = 4,
        ACTIVATE = 5,
        CLICKSKIPPED = 6,
        KEYSKIPPED = 7,
        SYSCOMMAND = 8,
        SETFOCUS = 9
    }

    public enum SIZE : int
    {
        SIZE_RESTORED = 0,
        SIZE_MINIMIZED = 1,
        SIZE_MAXIMIZED = 2,
        SIZE_MAXSHOW = 3,
        SIZE_MAXHIDE = 4,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int flags;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public static class Win32Window
    {
        public delegate bool EnumWindowProc(IntPtr hwnd, int lParam);

        public static uint RegisterWindow(string message)
        {
            return NativeMethods.RegisterWindowMessage(message);
        }

        public static SW GetWindowState(IntPtr handle)
        {
            NativeMethods.WINDOWPLACEMENT placement = new NativeMethods.WINDOWPLACEMENT();
            placement.Length = Marshal.SizeOf(placement);
            NativeMethods.GetWindowPlacement(handle, ref placement);

            SW state = (SW)placement.ShowCmd;
            return state;
        }

        public static bool ShowWindowAsync(IntPtr handle, SW showState)
        {
            return NativeMethods.ShowWindowAsync((int)handle, (int)showState);
        }

        public static IntPtr SetWindowLong(IntPtr handle, int option, int newValue)
        {
            return SetWindowLong(handle, option, new IntPtr(newValue));
        }

        public static IntPtr SetWindowLong(IntPtr handle, int option, IntPtr newValue)
        {
            if (IntPtr.Size == 8) // For 64 bit platforms
                return NativeMethods.SetWindowLongPtr(handle, option, newValue);
            else
                return new IntPtr(NativeMethods.SetWindowLong(handle, option, newValue.ToInt32()));
        }

        public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8) // For 64 bit platforms
                return NativeMethods.GetWindowLongPtr(hWnd, nIndex);
            else
                return new IntPtr(NativeMethods.GetWindowLong(hWnd, nIndex));
        }

        public static bool MoveWindow(IntPtr handle, double x, double y, double width, double height, bool repaint)
        {
            return MoveWindow(handle, (int)x, (int)y, (int)width, (int)height, repaint);
        }

        public static bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool repaint)
        {
            return NativeMethods.MoveWindow(handle, x, y, width, height, repaint);
        }

        public static IntPtr FindWindow(string className, string windowName)
        {
            return NativeMethods.FindWindow(className, windowName);
        }

        public static bool SetForegroundWindow(IntPtr handle)
        {
            return NativeMethods.SetForegroundWindow(handle);
        }

        public static string GetWindowTitle(IntPtr handle)
        {
            int length = NativeMethods.GetWindowTextLength(handle);
            StringBuilder title = new StringBuilder(length + 1);
            NativeMethods.GetWindowText(handle, title, title.Capacity);
            return title.ToString();
        }

        public static bool BringWindowToTop(IntPtr handle)
        {
            return NativeMethods.BringWindowToTop(handle);
        }

        public static bool SetWindowPos(IntPtr handle, IntPtr insertAfter, int left, int top, int right, int bottom, SWP flags)
        {
            return NativeMethods.SetWindowPos(handle, insertAfter, left, top, right, bottom, flags);
        }

        /// <summary>
        /// Changes the z-order of the window without activating it.
        /// </summary>
        public static bool ChangeWindowZOrder(IntPtr handle, HWND insertAfter)
        {
            return NativeMethods.SetWindowPos(handle, (IntPtr)insertAfter, 0, 0, 0, 0, SWP.NOSIZE | SWP.NOMOVE | SWP.NOACTIVATE);
        }

        public static IntPtr GetForegroundWindow()
        {
            return NativeMethods.GetForegroundWindow();
        }        

        public static void SwitchToThisWindow(IntPtr handle)
        {
            NativeMethods.SwitchToThisWindow(handle, false);
        }

        public static uint GetWindowThreadProcessId(IntPtr handle)
        {
            return NativeMethods.GetWindowThreadProcessId(handle, IntPtr.Zero);
        }

        public static IntPtr GetDesktopWindow()
        {
            return NativeMethods.GetDesktopWindow();
        }

        public static IntPtr GetShellWindow()
        {
            return NativeMethods.GetShellWindow();
        }

        public static RECT GetWindowRect(IntPtr handle)
        {
            RECT rect = new RECT();
            NativeMethods.GetWindowRect(handle, out rect);
            return rect;
        }

        public static bool IsWindowTopMost(IntPtr handle)
        {
            return ( (GetWindowLong(handle, (int)GWL.EXSTYLE)).ToInt32() & (int)WS_EX.TOPMOST ) != 0;
        }

        public static void EnumWindows(EnumWindowProc callback)
        {
            NativeMethods.EnumWindows(callback, 0);
        }

        public static bool EnumDesktopWindows(EnumWindowProc callback)
        {
            return NativeMethods.EnumDesktopWindows(IntPtr.Zero, callback, IntPtr.Zero);
        }

        public static WINDOWINFO GetWindowInfo(IntPtr handle)
        {
            WINDOWINFO info = new WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            NativeMethods.GetWindowInfo(handle, ref info);
            return info;
        }

        public static List<WS> TranslateWindowStyles(uint windowStyles)
        {
            List<WS> styles = new List<WS>();
            foreach (WS style in Enum.GetValues(typeof(WS)))
            {
                if (BitHelper.IsSet(windowStyles, (uint)style))
                    styles.Add(style);
            }

            return styles;
        }

        public static List<WS_EX> TranslateExtendedWindowStyles(uint extendedWindowStyles)
        {
            List<WS_EX> styles = new List<WS_EX>();
            foreach(WS_EX style in Enum.GetValues(typeof(WS_EX)))
            {
                if (BitHelper.IsSet(extendedWindowStyles, (uint)style))
                    styles.Add(style);                
            }

            return styles;
        }

        public static IntPtr SetWindowHookByThreadId(uint threadId, HOOKTYPE type, HOOKPROC callback)
        {
            return NativeMethods.SetWindowsHookEx(type, callback, IntPtr.Zero, threadId);        
        }
        
        public static IntPtr SetWindowHookByModule(IntPtr module, HOOKTYPE type, HOOKPROC callback)
        {
            return NativeMethods.SetWindowsHookEx(type, callback, module, 0);
        }

        public static void UnhookWindowsHookEx(IntPtr hookHandle)
        {
            NativeMethods.UnhookWindowsHookEx(hookHandle);
        }

        public static int CallNextHookEx(int code, IntPtr wParam, IntPtr lParam)
        {
            return NativeMethods.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        /// <summary>
        /// Checks to see if a full screen application is running. This function is only available on Windows Vista or later. 
        /// </summary>
        /// <remarks>http://www.pinvoke.net/default.aspx/shell32.shqueryusernotificationstate</remarks>
        public static bool? IsFullScreen()
        {
            if (Win32.IsFunctionAvailable("shell32.dll", "SHQueryUserNotificationState"))
            {
                QUERY_USER_NOTIFICATION_STATE state;
                NativeMethods.SHQueryUserNotificationState(out state);
                Debug.WriteLine(state);
                return (state & QUERY_USER_NOTIFICATION_STATE.QUNS_RUNNING_D3D_FULL_SCREEN) == QUERY_USER_NOTIFICATION_STATE.QUNS_RUNNING_D3D_FULL_SCREEN;
            }
            return null;
        }

        private static class NativeMethods
        {            
            /// <summary>
            /// Contains information about the placement of a window on the screen.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWPLACEMENT
            {
                /// <summary>
                /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
                /// <para>
                /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
                /// </para>
                /// </summary>
                public int Length;

                /// <summary>
                /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
                /// </summary>
                public int Flags;

                /// <summary>
                /// The current show state of the window.
                /// </summary>
                public SW ShowCmd;

                /// <summary>
                /// The coordinates of the window's upper-left corner when the window is minimized.
                /// </summary>
                public POINT MinPosition;

                /// <summary>
                /// The coordinates of the window's upper-left corner when the window is maximized.
                /// </summary>
                public POINT MaxPosition;

                /// <summary>
                /// The window's coordinates when the window is in the restored position.
                /// </summary>
                public RECT NormalPosition;

                /// <summary>
                /// Gets the default (empty) value.
                /// </summary>
                public static WINDOWPLACEMENT Default
                {
                    get
                    {
                        WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                        result.Length = Marshal.SizeOf(result);
                        return result;
                    }
                }
            }            

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(int hWnd, int nCmdShow);
            
            [DllImport("user32.dll")]
            public static extern bool ShowWindowAsync(int hWnd, int nCmdShow);
            
            /// <summary>
            /// Brings the thread that created the specified window into the 
            /// foreground and activates the window.
            /// </summary>
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
            
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
            
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern uint RegisterWindowMessage(string msg);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist", 
                Justification="It exists on 64 bit platforms and is only called on 64 bit platforms")]
            [DllImport("user32.dll")]
            public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
            
            [DllImport("user32.dll")]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist",
                Justification = "It exists on 64 bit platforms and is only called on 64 bit platforms")]
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool repaint);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool BringWindowToTop(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SWP uFlags);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

            // http://msdn.microsoft.com/en-us/library/ms633553(VS.85).aspx
            [DllImport("user32.dll", SetLastError = true)]
            public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetShellWindow();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowRect(IntPtr hwnd, out RECT rc);

            [DllImport("user32.dll")]
            public static extern IntPtr GetTopWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindowVisible(IntPtr hWnd);
            
            [DllImport("user32")]
            public static extern int EnumWindows(EnumWindowProc x, int y);

            [DllImport("user32.dll")]
            public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumWindowProc lpfn, IntPtr lParam);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(HOOKTYPE hookType, HOOKPROC lpfn, IntPtr hModule, uint dwThreadId);

            [DllImport("user32.dll")]
            public static extern int UnhookWindowsHookEx(IntPtr hhook);

            [DllImport("user32.dll")]
            public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            /// <summary>
            /// Windows Vista and greater only
            /// </summary>
            /// <param name="pquns"></param>
            /// <returns></returns>
            [DllImport("shell32.dll")]
            public static extern int SHQueryUserNotificationState(out QUERY_USER_NOTIFICATION_STATE pquns);
        }
    }
}
