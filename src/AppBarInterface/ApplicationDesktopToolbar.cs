using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.Win32Interface;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AstoundingApplications.AppBarInterface
{
    /// <summary>
    /// Application Desktop Toolbar interface
    /// </summary>
    /// <remarks>http://msdn.microsoft.com/en-us/library/cc144177(v=vs.85).aspx</remarks>
    public class ApplicationDesktopToolbar
    {
        public event AppBarEventHandler ToolbarEvent = delegate { };

        IntPtr _handle;
        uint _callback;

        public ApplicationDesktopToolbar(IntPtr handle)
        {
            _handle = handle;
            _callback = Win32Window.RegisterWindow(Guid.NewGuid().ToString());

            // Application Desktop Toolbars must have the WS_EX_TOOLWINDOW style in order to work correctly.
            // http://support.microsoft.com/kb/132965            
            SetToolWindowStyle(_handle); // TODO: I commented this out for some reason, why??

            // TODO: Hoping this will help 'on startup'.
            Win32Window.SetWindowPos(_handle, IntPtr.Zero, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE);
        }

        /// <summary>
        /// Registers a new appbar.
        /// </summary>
        /// <returns>Returns true if successful, otherwise false</returns>
        public bool Register()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;
            msgData.uCallbackMessage = _callback;
            
            return Win32AppBar.SHAppBarMessage(ABM.ABM_NEW, ref msgData).ToInt32() == Win32.TRUE;
        }

        /// <summary>
        /// Unregisters an appbar.
        /// </summary>
        public void Unregister()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;

            Win32AppBar.SHAppBarMessage(ABM.ABM_REMOVE, ref msgData);
        }

        /// <summary>
        /// Retrieves the handle to the appbar associated to the edge of the screen.
        /// </summary>
        /// <returns>
        /// Returns the handle to the autohide appbar. The return value is 0 if an error occurs or 
        /// if no autohide appbar is associated with the given edge.
        /// </returns>
        public IntPtr GetAutoHide(DockPosition dock)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;
            msgData.uEdge = (uint)dock.ToNative();

            return Win32AppBar.SHAppBarMessage(ABM.ABM_GETAUTOHIDEBAR, ref msgData);
        }

        /// <summary>
        /// Registers or unregisters an autohide appbar for an edge of the screen.
        /// </summary>
        /// <returns>
        /// Returns true if successful, or false if an error occurs or         
        /// if an autohide appbar is already registered for the given edge.
        /// </returns>
        public bool SetAutoHide(DockPosition dock, bool autohide)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;
            msgData.lParam = autohide ? Win32.TRUE : Win32.FALSE;
            msgData.uEdge = (uint)dock.ToNative();

            return Win32AppBar.SHAppBarMessage(ABM.ABM_SETAUTOHIDEBAR, ref msgData).ToInt32() == Win32.TRUE;
        }

        /// <summary>
        /// <para>
        ///     Requests a size and screen position for an appbar. When the request is made, the message proposes a 
        ///     screen edge and a bounding rectangle for the appbar. 
        /// </para>
        /// <para>
        ///     The system adjusts the bounding rectangle so 
        ///     that the appbar does not interfere with the Windows taskbar or any other appbars.
        /// </para>
        /// </summary>
        public void QueryPosition(ref Position position, DockPosition dock)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;
            msgData.uEdge = (uint)dock.ToNative();
            position.ToNative(ref msgData.rc);

            Win32AppBar.SHAppBarMessage(ABM.ABM_QUERYPOS, ref msgData);
            position = Position.FromNative(ref msgData.rc);
        }

        /// <summary>
        /// <para>
        ///     Sets the size and screen position of an appbar. The message specifies a 
        ///     screen edge and the bounding rectangle for the appbar. 
        /// </para>
        /// <para>
        ///     The system may adjust the bounding rectangle so that the appbar does not 
        ///     interfere with the Windows taskbar or any other appbars.
        /// </para>
        /// </summary>
        public void SetPosition(ref Position position, DockPosition dock)
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;
            msgData.uEdge = (uint)dock.ToNative();
            position.ToNative(ref msgData.rc);

            Win32AppBar.SHAppBarMessage(ABM.ABM_SETPOS, ref msgData);
            position = Position.FromNative(ref msgData.rc);
        }

        public Position GetTaskbarPosition()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);

            Win32AppBar.SHAppBarMessage(ABM.ABM_GETTASKBARPOS, ref msgData);
            return Position.FromNative(ref msgData.rc);
        }

        /// <summary>
        /// Notifies the system that an appbar has been activated. 
        /// </summary>
        void OnActivate()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;

            Win32AppBar.SHAppBarMessage(ABM.ABM_ACTIVATE, ref msgData);
        }

        /// <summary>
        /// Notifies the system when an appbar's position has changed. 
        /// </summary>
        void OnWindowPositionChanged()
        {
            APPBARDATA msgData = new APPBARDATA();
            msgData.cbSize = Marshal.SizeOf(msgData);
            msgData.hWnd = _handle;

            Win32AppBar.SHAppBarMessage(ABM.ABM_WINDOWPOSCHANGED, ref msgData);
        }

        //[DebuggerStepThrough]
        public IntPtr ProcessMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == _callback)
            {
                Debug.WriteLine((ABN)wParam.ToInt32());

                // Messages sent directly to the AppBar
                switch ((ABN)wParam.ToInt32())
                {
                    case ABN.ABN_POSCHANGED:
                        ToolbarEvent(this, new AppBarEventArgs(AppBarNotificationAction.PositionChanged));
                        break;
                    case ABN.ABN_FULLSCREENAPP:
                        // lParam = 1 (true)  if a full screen app has been opened.
                        // lParam = 0 (false) if a full screen app has been closed.
                        ToolbarEvent(this, new AppBarEventArgs()
                            {
                                Action = AppBarNotificationAction.FullScreenApp,
                                Data = lParam.ToInt32() == Win32.TRUE
                            });
                        break;
                    case ABN.ABN_STATECHANGE:
                        ToolbarEvent(this, new AppBarEventArgs(AppBarNotificationAction.StateChanged));
                        break;
                    case ABN.ABN_WINDOWARRANGE:
                        // lParam = 1 (true)  if the window arrange event is about to occur
                        // lParam = 0 (false) if the window arrange event has just occurred
                        ToolbarEvent(this, new AppBarEventArgs()
                            {
                                Action = AppBarNotificationAction.WindowArranged,
                                Data = lParam.ToInt32() == Win32.TRUE
                            });
                        break;
                }
            }
            else
            {
                Debug.WriteLine((WM)msg);

                // Messages sent by/to other windows
                switch ((WM)msg)
                {
                    case WM.WM_ACTIVATE:
                        OnActivate();
                        break;
                    case WM.WM_WINDOWPOSCHANGED:
                        OnWindowPositionChanged();
                        break;
                    //case WM.WM_GETMINMAXINFO:
                        //OnWindowPositionChanged();
                        //break;
                    //case WM.WM_WINDOWPOSCHANGING:
                        //OnWindowPositionChanged();
                        //break;
                    case WM.WM_WININICHANGE:
                        Debug.Print("WM_WININICHANGE lParam {0}, wParam {1}", lParam, (SPI)wParam);
                        if ((SPI)wParam == SPI.SPI_SETWORKAREA)
                        {
                            ToolbarEvent(this, new AppBarEventArgs(AppBarNotificationAction.PositionChanged));
                        }
                        break;
                    case WM.WM_DISPLAYCHANGE:
                        //ToolbarEvent(this, new AppBarEventArgs(AppBarNotificationAction.PositionChanged));
                        break;
                    case WM.WM_NCHITTEST:
                        // This should be handled if you want to implement drag resizing.
                        break;
                }
            }

            return IntPtr.Zero;
        }

        /// <summary> Gives the window the WS_EX_TOOLWINDOW window style. </summary>
        void SetToolWindowStyle(IntPtr handle)
        {           
            int currentStyle = (int)Win32Window.GetWindowLong(handle, (int)GWL.EXSTYLE);
            Win32Window.SetWindowLong(handle, (int)GWL.STYLE, currentStyle | (int)WS_EX.TOOLWINDOW);
        }
    }
}
