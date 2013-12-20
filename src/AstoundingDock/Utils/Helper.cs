using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AstoundingApplications.AstoundingDock.Utils
{
    static class Helper
    {
        public static Screen GetScreenFromName(string name)
        {
            Screen screen = Screen.AllScreens.FirstOrDefault(
                    obj => obj.DeviceName == name);
            return screen ?? Screen.PrimaryScreen;
        }

        public static int IconSizeToInt(IconSize iconSize)
        {
            switch (iconSize)
            {
                case IconSize.Small:
                    return ApplicationIcon.Small;
                case IconSize.Medium:
                    return ApplicationIcon.Medium;                
                case IconSize.Large:
                    return ApplicationIcon.Large;
                default:
                    throw new NotImplementedException(iconSize.ToString());
            }
        }

        public static string[] SplitCamelCase(string camelCaseString)
        {
            if (camelCaseString == null)
                return null;

            return System.Text.RegularExpressions.Regex.Replace(camelCaseString,
                "([A-Z])",
                " $1",
                System.Text.RegularExpressions.RegexOptions.Compiled
                ).Trim().Split(' ');
        }
    }
}
