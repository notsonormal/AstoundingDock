using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AstoundingApplications.AppBarInterface
{
    public delegate void AppBarEventHandler(object sender, AppBarEventArgs e);
    public enum AppBarNotificationAction { PositionChanged, FullScreenApp, StateChanged, WindowArranged, ShowWindow, HideWindow, ErrorEvent }

    public class AppBarEventArgs : EventArgs
    {
        public AppBarNotificationAction Action { get; set; }
        public object Data { get; set; }

        public AppBarEventArgs() { }
        public AppBarEventArgs(AppBarNotificationAction action) : this(action, null) { }
        public AppBarEventArgs(AppBarNotificationAction action, object data)
        {
            Action = action;
            Data = data;
        }
    }

    public class AppBarDefaultValues
    {
        public static readonly Screen Screen = Screen.PrimaryScreen;
        public static readonly int PopupDelay = 1000;
        public static readonly int AutoHideDelay = 1000;
        public static readonly bool AutoHide = false;
        public static readonly bool ReserveScreen = false;
        public static readonly DockEdge Docked = DockEdge.Left;
    }

    public interface IAppBar
    {
        event AppBarEventHandler AppBarEvent;

        System.Windows.Forms.Screen ActiveScreen { get; set; }
        int PopupDelay { get; set; }
        bool IsFullScreenAppOpen { get; }
        int AutoHideDelay { get; set; }
        bool ReserveScreen { get; set; }
        bool Topmost { get; set;  }
        IntPtr Handle { get; }
        bool Dragging { get; }

        void Init();
        void Register();
        void Unregister();
        void Reserve();
        void Unreserve();
        void CorrectPosition(ref Position position);
        void Resize(ref Position position);
        void Dock(DockPosition dock);
        void SetPosition(ref Position position, DockPosition dock);
        IntPtr GetAutoHide(DockPosition dock);
        void SetAutoHide(DockPosition dock, bool autohide);
        void Dispose();
    }
}
