using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class ShellContextMenuMessage
    {
        public bool IsOpen { get; private set; }

        public static ShellContextMenuMessage Opened()
        {
            return new ShellContextMenuMessage()
            {
                IsOpen = true
            };
        }

        public static ShellContextMenuMessage Closed()
        {
            return new ShellContextMenuMessage()
            {
                IsOpen = false
            };
        }
    }
}
