using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.ViewModels;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class ExpandedTabMessage
    {
        public TabViewModel ExpandedThis { get; private set; }

        public ExpandedTabMessage(TabViewModel expandThis)
        {
            ExpandedThis = expandThis;
        }
    }
}
