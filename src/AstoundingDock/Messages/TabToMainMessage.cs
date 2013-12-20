using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.ViewModels;

namespace AstoundingApplications.AstoundingDock.Messages
{
    class TabToMainMessage
    {
        public enum ActionType { AddTab, AddApplication, ViewApplications, OpenSettings, OpenAbout, Close }

        public ActionType Action { get; private set; }

        public static TabToMainMessage AddTab()
        {
            return new TabToMainMessage()
            {
                Action = ActionType.AddTab,
            };
        }

        public static TabToMainMessage AddApplication()
        {
            return new TabToMainMessage()
            {
                Action = ActionType.AddApplication,
            };
        }

        public static TabToMainMessage ViewApplication()
        {
            return new TabToMainMessage()
            {
                Action = ActionType.ViewApplications,
            };
        }

        public static TabToMainMessage OpenSettings()
        {
            return new TabToMainMessage()
            {
                Action = ActionType.OpenSettings,
            };
        }

        public static TabToMainMessage OpenAbout()
        {
            return new TabToMainMessage()
            {
                Action = ActionType.OpenAbout,
            };
        }

        public static TabToMainMessage Close()
        {
            return new TabToMainMessage()
            {
                Action = ActionType.Close,
            };
        }
    }
}
