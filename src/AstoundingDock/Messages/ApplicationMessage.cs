using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.ViewModels;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class ApplicationMessage
    {
        public enum ActionType { Add, AddNew, Remove, Move, Edit }

        public ActionType Action { get; private set; }
        public ApplicationViewModel Application { get; private set; }
        public TabViewModel Tab { get; private set; }
        public bool SuppressMessage { get; private set; }

        public static ApplicationMessage Add(ApplicationViewModel addThis, TabViewModel addToThis)
        {
            return Add(addThis, addToThis, false);
        }

        public static ApplicationMessage Add(ApplicationViewModel addThis, TabViewModel addToThis, bool suppressMessage)
        {
            return new ApplicationMessage()
            {
                Action = ActionType.Add,
                Application = addThis,
                Tab = addToThis,
                SuppressMessage = suppressMessage
            };
        }

        public static ApplicationMessage AddNew()
        {
            return new ApplicationMessage()
            {
                Action = ActionType.AddNew,
            };
        }

        public static ApplicationMessage Edit(ApplicationViewModel editThis)
        {
            return new ApplicationMessage()
            {
                Action = ActionType.Edit,
                Application = editThis
            };
        }

        public static ApplicationMessage Move(ApplicationViewModel moveThis, TabViewModel moveToThis)
        {
            return new ApplicationMessage()
            {
                Action = ActionType.Move,
                Application = moveThis,
                Tab = moveToThis
            };
        }

        public static ApplicationMessage Remove(ApplicationViewModel removeThis)
        {
            return new ApplicationMessage()
            {
                Action = ActionType.Remove,
                Application = removeThis
            };
        }
    }
}
