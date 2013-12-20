using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.ViewModels;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class TabMessage
    {
        public enum ActionType { Add, Remove, Edit, Move }

        public ActionType Action { get; private set; }
        public TabViewModel Tab { get; private set; }
        public TabViewModel TabB { get; private set; }

        public static TabMessage Add(TabViewModel addThis)
        {
            return new TabMessage()
            {
                Action = ActionType.Add,
                Tab = addThis
            };
        }

        public static TabMessage Remove(TabViewModel removeThis)
        {
            return new TabMessage()
            {
                Action = ActionType.Remove,
                Tab = removeThis
            };
        }

        public static TabMessage Edit(TabViewModel editThis)
        {
            return new TabMessage()
            {
                Action = ActionType.Edit,
                Tab = editThis
            };
        }

        public static TabMessage Move(TabViewModel moveThis, TabViewModel moveToThis)
        {
            return new TabMessage()
            {
                Action = ActionType.Move,
                Tab = moveThis,
                TabB = moveToThis
            };
        }
    }
}
