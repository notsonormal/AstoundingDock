using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class MenuMessage
    {
        public bool IsOpen { get; private set; }

        public static MenuMessage Opened()
        {
            return new MenuMessage()
            {
                IsOpen = true
            };
        }

        public static MenuMessage Closed()
        {
            return new MenuMessage()
            {
                IsOpen = false
            };
        }
    }
}
