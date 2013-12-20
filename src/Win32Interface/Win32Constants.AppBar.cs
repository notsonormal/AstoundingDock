using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.Win32Interface
{
    public static partial class Win32Constants
    {
        #region AppBar Edges
        public const int ABE_LEFT = 0;
        public const int ABE_TOP = 1;
        public const int ABE_RIGHT = 2;
        public const int ABE_BOTTOM = 3;
        #endregion

        #region AppBar Messages
        public const int ABM_NEW = 0;
        public const int ABM_REMOVE = 1;
        public const int ABM_QUERYPOS = 2;
        public const int ABM_SETPOS = 3;
        public const int ABM_GETSTATE = 4;
        public const int ABM_GETTASKBARPOS = 5;
        public const int ABM_ACTIVATE = 6;
        public const int ABM_GETAUTOHIDEBAR = 7;
        public const int ABM_SETAUTOHIDEBAR = 8;
        public const int ABM_WINDOWPOSCHANGED = 9;
        public const int ABM_SETSTATE = 10;
        #endregion

        #region AppBar States
        public const int ABS_NEITHER = 0x00000000;
        public const int ABS_AUTOHIDE = 0x00000001;
        public const int ABS_ALWAYSONTOP = 0x00000002;
        #endregion
    }
}
