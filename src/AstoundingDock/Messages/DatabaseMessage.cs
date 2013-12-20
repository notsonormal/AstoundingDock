using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.Models;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class DatabaseMessage
    {
        public enum ActionType { Add, Update, Remove, Move, Save }

        public ActionType Action { get; private set; }
        public TabModel Tab { get; private set; }
        public TabModel TabB { get; private set; }
        public ApplicationModel Application { get; private set; }

        public static DatabaseMessage Add(TabModel addThis)
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Add,
                Tab = addThis
            };
        }

        public static DatabaseMessage Add(ApplicationModel addThis)
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Add,
                Application = addThis
            };
        }

        public static DatabaseMessage Update(TabModel updateThis)
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Update,
                Tab = updateThis
            };
        }

        public static DatabaseMessage Update(ApplicationModel updateThis)
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Update,
                Application = updateThis
            };
        }

        public static DatabaseMessage Remove(TabModel updateThis)
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Remove,
                Tab = updateThis
            };
        }

        public static DatabaseMessage Remove(ApplicationModel updateThis)
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Remove,
                Application = updateThis
            };
        }

        public static DatabaseMessage Move(TabModel moveThis, TabModel moveToThis)
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Move,
                Tab = moveThis,
                TabB = moveToThis
            };
        }

        public static DatabaseMessage Save()
        {
            return new DatabaseMessage()
            {
                Action = ActionType.Save
            };
        }        
    }
}
