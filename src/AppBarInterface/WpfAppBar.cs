using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using AstoundingApplications.Win32Interface;
using System.Timers;
using System.Diagnostics;
using System.ComponentModel;

namespace AstoundingApplications.AppBarInterface
{
    public class WpfAppBar : IAppBar, IDisposable
    {
        public event AppBarEventHandler AppBarEvent = delegate { };

        #region Fields
        HwndSource _source;
        ApplicationDesktopToolbar _toolbar;
        RawInput _rawInput;
        bool _registered;        
        int _autoHideDelay;
        int _popupDelay;
        Timer _showTimer;
        Timer _hideTimer;
        Position _currentPosition;
        Window _window;
        #endregion

        public WpfAppBar(Window window)
        {
            _currentPosition = new Position();
            _window = window;

            // Show/Hide timers.
            _showTimer = new Timer();
            _showTimer.AutoReset = false;
            _showTimer.Elapsed += OnShowTimerElapsed;

            _hideTimer = new Timer();
            _hideTimer.AutoReset = true;
            _hideTimer.Elapsed += OnHideTimerElapsed;

            // Windows styles and events.
            _window.WindowStyle = WindowStyle.None;
            _window.ResizeMode = ResizeMode.NoResize;
            _window.ShowInTaskbar = false;

            _window.Closed += OnClose;
            _window.MouseLeave += OnMouseLeave;

            _window.AllowDrop = true;
            _window.PreviewDragEnter += OnDragEnter;
            _window.DragLeave += OnDragLeave;

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;

            // Default Values.
            PopupDelay = AppBarDefaultValues.PopupDelay;
            AutoHideDelay = AppBarDefaultValues.AutoHideDelay;
            ReserveScreen = AppBarDefaultValues.ReserveScreen;
            Docked = new DockPosition(AppBarDefaultValues.Docked);
            ActiveScreen = AppBarDefaultValues.Screen;        
        }        

        #region Properties
        public DockPosition Docked { get; private set; }
        public System.Windows.Forms.Screen ActiveScreen { get; set; }
        public int AutoHideDelay
        {
            get { return _autoHideDelay; }
            set
            {
                _autoHideDelay = value;
                _hideTimer.Interval = value;
            }
        }
        public int PopupDelay
        {
            get { return _autoHideDelay; }
            set
            {
                _popupDelay = value;
                _showTimer.Interval = value;
            }
        }
        public bool ReserveScreen { get; set; }
        public bool IsFullScreenAppOpen { get; set; }
        public bool Topmost
        {
            get
            {
                bool topmost = false;
                //Win32Window.SetWindowPos(Handle, (IntPtr)HWND.BOTTOM, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE);
                _window.Dispatcher.Invoke(new Action(() => topmost = _window.Topmost ));
                return topmost;
            }
            set
            {                
                //Win32Window.SetWindowPos(Handle, (IntPtr)HWND.BOTTOM, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE);                   
                _window.Dispatcher.Invoke(new Action(() => _window.Topmost = value));                
            }
        }
        public IntPtr Handle { get; private set; }
        public bool Dragging { get; private set; }
        #endregion

        public void Init()
        {
            // Win32 interface. 
    
            // TODO: Can these three lines be moved to contructor?
            // and maybe the raw input ones too?
            Handle = new WindowInteropHelper(_window).Handle;
            _toolbar = new ApplicationDesktopToolbar(Handle);
            _toolbar.ToolbarEvent += OnToolbarEvent;

            // Raw input.
            try
            {
                _rawInput = new RawInput(Handle);
                _rawInput.RawInputEvent += OnRawInputEvent;
            }
            catch (Win32Exception ex)
            {
                // NOTE: This error is probably caused by the fact that you are logged in via remote desktop, 
                // raw input doesn't seem to be able to find the mouse point in that cause. 
                // This is non-blocking as it is only required for auto hidding

                // TODO: There may be some sort of solution to this. For example the problem might be
                // that I just need to periodically query for new devices.
                AppBarEvent(this, new AppBarEventArgs()
                    {
                        Action = AppBarNotificationAction.ErrorEvent,
                        Data = String.Format("Problem tring to register for raw input events, {0}", ex.Message),
                    });                
            }

            // Listen for window events.
            _source = HwndSource.FromHwnd(Handle);
            if (_source == null)
                throw new AppBarException("Window is not loaded yet");
            _source.AddHook(new HwndSourceHook(WndProc));

            Register();
        }

        public void Register()
        {
            if (_toolbar == null)
                return;

            if (!_registered)
            {
                if (!_toolbar.Register())
                    throw new AppBarException("Failed to register appbar");
                _registered = true;                
            }
        }

        public void Unregister()
        {
            if (_toolbar == null)
                return;

            if (_registered)
            {
                _toolbar.Unregister();
                _registered = false;
            }
        }

        public void Reserve()
        {
            Topmost = true;            
            ReserveScreen = true;
            SetNormal();
        }

        public void Unreserve()
        {
            Topmost = false;
            ReserveScreen = false;
            SetMinimized();
        }        

        public void SetPosition(ref Position position, DockPosition dock)
        {
            Dock(dock);
            CorrectPosition(ref position);
            Resize(ref position);           
        }

        public void CorrectPosition(ref Position position)
        {
            if (_toolbar != null)
            {
                _toolbar.QueryPosition(ref position, Docked);

                // The query position call will sometimes set the height or width 
                // to a negative value if the app bar is moved to the same side as the taskbar.

                // Obviously that's going to cause problems so set them to 0 in that situation.
                if (position.Height < 0)
                    position.Height = 0;
                if (position.Width < 0)
                    position.Width = 0;

                _currentPosition = position;
            }
        }

        public void Resize(ref Position position)
        {
            _currentPosition = position;
            if (ReserveScreen)
                SetNormal();
            else
                SetMinimized();
        }

        void SetNormal()
        {
            if (_toolbar == null)
                return; 

            _toolbar.SetPosition(ref _currentPosition, Docked);
        }

        void SetMinimized()
        {
            if (_toolbar == null)
                return;

            Position position = new Position();
            _toolbar.SetPosition(ref position, DockPosition.Left());
        }

        public void Dock(DockPosition dock)
        {
            if (Docked != dock)
            {
                SetAutoHide(Docked, false);
            }

            Docked = dock;
            SetAutoHide(Docked, true);
        }

        public void SetAutoHide(DockPosition dock, bool autoHide)
        {
            if (_toolbar == null)
                return;

            bool success = _toolbar.SetAutoHide(dock, autoHide);
            if (!success && autoHide)
            {
                //throw new AppBarException("Unable to set autohide on that edge, an autohide bar has already been set");
                Debug.WriteLine("Unable to set autohide on that edge, an autohide bar has already been set");
            }
        }

        public IntPtr GetAutoHide(DockPosition dock)
        {
            if (_toolbar == null)
                return IntPtr.Zero;

            return _toolbar.GetAutoHide(dock);
        }

        void OnToolbarEvent(object sender, AppBarEventArgs e)
        {
            Debug.Print("OnToolbarEvent - Action: {0}, Data: {1}, ReverseScreen: {2}, Win32Window.IsFullScreen {3}", e.Action, e.Data, ReserveScreen, Win32Window.IsFullScreen());

            switch (e.Action)
            {
                case AppBarNotificationAction.FullScreenApp:
                    IsFullScreenAppOpen = (bool)e.Data;                    

                    if (IsFullScreenAppOpen && ReserveScreen)
                    {                        
                        //Topmost = false;
                        //Win32Window.SetWindowPos(Handle, (IntPtr)HWND.BOTTOM, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE);
                    }
                    else if (!IsFullScreenAppOpen && ReserveScreen)
                    {
                        //Topmost = true;
                        //Win32Window.SetWindowPos(Handle, (IntPtr)HWND.NOTOPMOST, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE | SWP.FRAMECHANGED);                                               
                    }
                    AppBarEvent(this, e);
                    break;
                default:
                    AppBarEvent(this, e);
                    break;
            }
        }

        void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            HideWindow();
        }

        void OnRawInputEvent(object sender, RawInputEventArgs e)
        {
            switch (e.Action)
            {
                case RawInputAction.MouseMove:
                    if (!IsFullScreenAppOpen && OverScreenEdge())
                    {
                        ShowWindow();
                    }
                    break;
            }
        }

        bool OverScreenEdge()
        {
            int offset = 5;
            int[] mousePosition = Win32Mouse.GetMousePosition();

            // Make sure the mouse point is inside the screen bounds.
            if (!ActiveScreen.WorkingArea.Contains(mousePosition[0], mousePosition[1]))
                return false;

            switch (Docked.Selected)
            {
                case DockEdge.Left:
                    if (mousePosition[0] <= ActiveScreen.WorkingArea.Left + offset)
                    {
                        // Close enough to the edge.
                        return true;
                    }
                    break;
                case DockEdge.Right:
                    if (mousePosition[0] >= ActiveScreen.WorkingArea.Right - offset)
                    {
                        // Close enough to the edge.
                        return true;
                    }
                    break;
                case DockEdge.Top:
                    if (mousePosition[1] <= ActiveScreen.WorkingArea.Top + offset)
                    {
                        // Close enough to the edge.
                        return true;
                    }
                    break;
                case DockEdge.Bottom:
                    if (mousePosition[1] >= ActiveScreen.WorkingArea.Bottom - offset)
                    {
                        // Close enough to the edge.
                        return true;
                    }
                    break;
            }

            return false;
        }

        /*
        void CheckForFullScreenApps()
        {
            IntPtr desktop = Win32Window.GetDesktopWindow();
            IntPtr shell = Win32Window.GetShellWindow();

            RECT appbarRect = Win32Window.GetWindowRect(Handle);
            Rect appbarBounds = new Rect(
                new Point(appbarRect.left, appbarRect.top), 
                new Point(appbarRect.right, appbarRect.bottom));

            Win32Window.EnumWindows((handle, param) =>
                {
                    if (handle != Handle && handle != desktop && handle != shell)
                    {
                        string title = Win32Window.GetWindowTitle(handle);
                        if (title.Contains("Windows XP"))
                        {
                            RECT rect = Win32Window.GetWindowRect(handle);
                            //Rect bounds = new Rect(
                            //    new Point(rect.left, rect.top),
                            //    new Point(rect.right, rect.bottom));

                            //System.Drawing.Rectangle bounds = new System.Drawing.Rectangle(
                            //    rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);

                            if (ActiveScreen.Bounds.Width == (rect.right - rect.left) &&
                                ActiveScreen.Bounds.Height == (rect.bottom - rect.top))
                            {
                                //SetMinimized();
                                //Unreserve();
                                //Win32Window.SetWindowPos(Handle, (IntPtr)HWND.BOTTOM, 0, 0, 0, 0, SWP.NOMOVE | SWP.NOSIZE | SWP.NOACTIVATE);
                                //Reserve();
                                //Win32Window.SetForegroundWindow(handle);
                                return false;
                            }
                        }
                    }

                    return true;
                });
        }
         */

        void OnDragEnter(object sender, DragEventArgs e)
        {
            Dragging = true;
        }

        void OnDragLeave(object sender, DragEventArgs e)
        {
            Dragging = false;
        }

        void ShowWindow()
        {
            if (!_showTimer.Enabled)            
                _showTimer.Start();            
        }

        void HideWindow()
        {
            if (_hideTimer.Enabled)
                _hideTimer.Stop();
            _hideTimer.Start();
        }

        void OnShowTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!IsFullScreenAppOpen && OverScreenEdge())
            {
                AppBarEvent(this, new AppBarEventArgs(AppBarNotificationAction.ShowWindow));
            }
        }

        void OnHideTimerElapsed(object sender, ElapsedEventArgs e)
        {
            /*
            if (ReserveScreen)
            {
                // TODO: Check if a 'FullScreen' app is overing the active window, if so,
                // push down to bottom.
                CheckForFullScreenApps();
                return;
            }
             */

            AppBarEvent(this, new AppBarEventArgs(AppBarNotificationAction.HideWindow));
        }

        void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            // Handle screen resolution changes
            AppBarEvent(this, new AppBarEventArgs(AppBarNotificationAction.PositionChanged));
        }

        void OnClose(object sender, EventArgs e)
        {
            Dispose();           
        }

        [DebuggerStepThrough]
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {            
            switch ((WM)msg)
            {
                case WM.WM_INPUT:
                    if (_rawInput != null)
                        _rawInput.ProcessInput(hwnd, msg, wParam, lParam, ref handled);
                    break;
                default:
                    _toolbar.ProcessMessage(hwnd, msg, wParam, lParam, ref handled);
                    break;
            }
            return IntPtr.Zero;
        }

        #region IDisposable Interface
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                // Dispose managed resources.
                if (disposing)
                {
                    _showTimer.Dispose();
                    _hideTimer.Dispose();
                }

                // Dispose unmanaged resources.                
                if (_source != null)                
                    _source.RemoveHook(WndProc);                                    

                if (_rawInput != null)
                    _rawInput.Dispose();

                if (_toolbar != null)
                {
                    _toolbar.SetAutoHide(Docked, false);
                    _toolbar.Unregister();
                }
            }
        }

        ~WpfAppBar()
        {
            Dispose(false);
        }
        #endregion
    }
}
