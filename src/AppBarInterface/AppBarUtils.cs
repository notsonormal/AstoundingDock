using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AstoundingApplications.Win32Interface;
using System.Drawing;
using System.Diagnostics;

namespace AstoundingApplications.AppBarInterface
{
    public static class AppBarUtils
    {
        public static bool IsForegroundWindowFullScreen(Screen screen)
        {
            IntPtr handle = Win32Window.GetForegroundWindow();
            return IsWindowFullScreen(handle, screen);
        }

        public static bool IsWindowFullScreen(IntPtr handle, Screen screen)
        {
            if (screen == null)
                screen = Screen.PrimaryScreen;

            RECT rect = Win32Window.GetWindowRect(handle);

            Debug.Print("IsWindowFullScreen, Left: {0}, Top: {1}, Right: {2}, Bottom: {3}", rect.left, rect.top, rect.right, rect.bottom);
            return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top).Contains(screen.Bounds);
        }
    }
}
