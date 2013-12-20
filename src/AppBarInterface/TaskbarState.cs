using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.Win32Interface;

namespace AstoundingApplications.AppBarInterface
{
    public class TaskbarState
    {
        public bool AlwaysOnTop { get; set; }
        public bool AutoHide { get; set; }

        internal static TaskbarState FromNative(uint state)
        {
            TaskbarState taskbarState = new TaskbarState();
            taskbarState.AlwaysOnTop = (state & (uint)ABS.ALWAYSONTOP) == (uint)ABS.ALWAYSONTOP;
            taskbarState.AutoHide = (state & (uint)ABS.AUTOHIDE) == (uint)ABS.AUTOHIDE;
            return taskbarState;
        }

        internal static void ToNative(ref bool alwaysOntop, ref bool autoHide)
        {
            throw new NotImplementedException();
        }
    }
}
