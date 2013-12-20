using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IWshRuntimeLibrary;
using System.IO;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace AstoundingApplications.AstoundingDock.Utils
{
    static class FileExtensions
    {
        /// <summary>.exe</summary>
        public const string Executable = ".exe";
        /// <summary>.lnk</summary>
        public const string Shortcut = ".lnk";
        /// <summary>.appref-ms</summary>
        public const string ClickOnce = ".appref-ms";
        /// <summary>.xml</summary>
        public const string Xml = ".xml";
        /// <summary>.jpg</summary>
        public const string Jpeg = ".jpg";
        /// <summary>.png</summary>
        public const string Png = ".png";
        /// <summary>.bmp</summary>
        public const string Bmp = ".bmp";
        /// <summary>.gif</summary>
        public const string Gif = ".gif";
        /// <summary>.ico</summary>
        public const string Icon = ".ico";
        /// <summary>.url</summary>
        public const string Url = ".url";
    }

    static class FileFilters
    {
        public const string ExecutableFiles = "Executable Files (*.exe, *.lnk)|*.exe;*.lnk";
        public const string ImageFiles = "Image Files (*.jpg, *.gif, *.png, *.bmp *.ico)|*.jpg;*.gif;*.png;*.bmp;*.ico";
        public const string XmlFiles = "Xml Files (*.xml)|*.xml;";
        public const string AllFiles = "All Files (*.*)|*.*;";
    }
    
    static class FileHelper
    {
        public static string GetSettingsDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "AstoundingDock");
        }

        public static string GetApplicationDirectory()
        {
            return Path.GetDirectoryName(GetApplicationExe());
        }

        public static string GetApplicationExe()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        public static string GetSteamIconsDirectory(string steamPath)
        {
            string steamDirectory = Path.GetDirectoryName(steamPath);
            return Path.Combine(Path.Combine(steamDirectory, "steam"), "games");
        }

        public static IWshShortcut ResolveShortcut(string shortcut)
        {           
            WshShell shell = new WshShell();
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcut);
            return link;
        }
    }
}
