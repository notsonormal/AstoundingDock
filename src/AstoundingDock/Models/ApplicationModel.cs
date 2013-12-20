using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.RegularExpressions;
using AstoundingApplications.AstoundingDock.Utils;

namespace AstoundingApplications.AstoundingDock.Models
{
    using PathEx = AstoundingApplications.AstoundingDock.Extensions.PathEx;
    using System.Drawing;
    using AstoundingApplications.Win32Interface;

    class ApplicationModel : IEquatable<ApplicationModel>
    {
        #region Fields
        string _title;
        string _oldTitle;
        string _tab;
        string _oldTab;
        string _filePath;
        #endregion Fields

        public ApplicationModel()
        {
        }

        public static ApplicationModel FromFile(string filePath, string tab)
        {
            return FromFile(filePath, tab, false);
        }

        public static ApplicationModel FromFile(string filePath, string tab, bool noExceptions)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath");
            if (String.IsNullOrWhiteSpace(tab))
                throw new ArgumentException("tab");

            string title = Path.GetFileNameWithoutExtension(filePath);
            string arguments = null;
            string icon = null;

            // If the file is a shortcut then get the real location of the file                             
            if (PathEx.HasExtension(filePath, FileExtensions.Shortcut))
            {
                // The windows installer can create it's own 'special' shortcuts. If that doesn't work
                // (i.e. the function returns null) then treat the file as a regular shortcut. 
                string msiPath = Win32Shortcut.ParseMsiShortcut(filePath);
                if (msiPath == null)
                {
                    var shortcut = FileHelper.ResolveShortcut(filePath);
                    icon = shortcut.IconLocation.Split(',')[0];
                    filePath = shortcut.TargetPath;
                    arguments = shortcut.Arguments;
                }
                else
                {
                    filePath = msiPath;
                }

                if (arguments != null && arguments.Length >= 10 && arguments.StartsWith("-applaunch"))
                {
                    // This file looks like it's a shortcut to a steam application.
                    int steamApplicationNumber = 0;
                    if (Int32.TryParse(arguments.Substring(10).Trim(), out steamApplicationNumber))
                    {
                        return FromSteamApp(title, steamApplicationNumber, icon, tab, noExceptions);
                    }
                }

                if (!File.Exists(filePath))
                {
                    if (noExceptions)
                    {
                        return null;
                    }
                    else
                    {
                        throw new FileNotFoundException(String.Format("The file the shortcut is linking to does not exist: '{0}'", filePath));
                    }
                }
            }

            if (PathEx.HasExtension(filePath, FileExtensions.Url))
            {
                ApplicationModel urlApplication = FromUrl(title, filePath, tab);
                if (urlApplication != null)
                    return urlApplication;
            }
            
            FileInfo file = new FileInfo(filePath);
            
            // Try to 'guess' where the uninstaller is
            string uninstaller = null;
            if (File.Exists(Path.Combine(file.DirectoryName, "unins000.exe")))
                uninstaller = Path.Combine(file.DirectoryName, "unins000.exe");
            else if (File.Exists(Path.Combine(file.DirectoryName, "uninst.exe")))
                uninstaller = Path.Combine(file.DirectoryName, "uninst.exe");
            else if (File.Exists(Path.Combine(file.DirectoryName, "Uninstall.exe")))
                uninstaller = Path.Combine(file.DirectoryName, "Uninstall.exe");
            else if (File.Exists(Path.Combine(file.DirectoryName, "uninstall.exe")))
                uninstaller = Path.Combine(file.DirectoryName, "uninstall.exe");
            else if (File.Exists(Path.Combine(file.DirectoryName, "uninstaller.exe")))
                uninstaller = Path.Combine(file.DirectoryName, "uninstaller.exe");

            ApplicationModel model = new ApplicationModel()
            {
                Title = title,
                FilePath = file.FullName,
                RunArguments = arguments,
                Tab = tab,
                ImagePath = icon,
                Installed = true,
                Uninstaller = uninstaller
            };

            return model;
        }

        public static ApplicationModel FromUrl(string title, string filePath, string tab)
        {
            string url = null;

            // Attempt to pull the url field from the file.
            using (TextReader reader = new StreamReader(filePath))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("URL="))
                    {
                        string[] splitLine = line.Split('=');
                        if (splitLine.Length > 0)
                        {
                            url = splitLine[1];
                            break;
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(url))
            {
                Match steamRegex = Regex.Match(url, "^steam://rungameid/([0-9]*)");
                if (steamRegex.Success)
                {
                    // Get the correct steam icon from the registry
                    string icon = RegistryHelper.GetSteamDisplayIcon(steamRegex.Groups[1].Value);

                    // Create the steam application model
                    return FromSteamApp(title, Int32.Parse(steamRegex.Groups[1].Value), icon, tab);
                }
                else
                {
                    // No other urls supported at the moment
                }                
            }
            
            return null;
        }

        public static ApplicationModel FromSteamApp(string title, int steamAppNumber, string icon, string tab)
        {
            return FromSteamApp(title, steamAppNumber, icon, tab, false);
        }

        public static ApplicationModel FromSteamApp(string title, int steamAppNumber, string icon, string tab, bool noExceptions)
        {
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentException("title");
            if (String.IsNullOrWhiteSpace(tab))
                throw new ArgumentException("tab");

            string steamPath = Configuration.SteamPath;
            if (!File.Exists(steamPath))
            {
                if (noExceptions)
                {
                    return null;
                }
                else
                {
                    throw new FileNotFoundException(String.Format(
                        "Cannot create steam application {0}, invalid steam path '{1}'", title, steamPath));
                }
            }

            ApplicationModel application = new ApplicationModel()
            {
                Title              = title,
                IsSteamApp         = true,
                FilePath           = steamPath,                
                SteamAppNumber     = steamAppNumber,
                RunArguments       = "-applaunch " + steamAppNumber,
                Installer          = steamPath,
                InstallArguments   = "steam://install/" + steamAppNumber,
                Uninstaller        = steamPath,
                UninstallArguments = "steam://uninstall/" + steamAppNumber,
                ImagePath          = icon,
                Installed          = true,                   
                Tab                = tab
            };
         
            return application;
        }        

        #region Properties
        public string Title
        {
            get { return _title; }
            set
            {
                _oldTitle = _title;
                _title = value;
            }
        }
        public string OldTitle { get { return _oldTitle ?? _title; } }
        public string ImagePath { get; set; }        
        public string FilePath 
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                ConfigureSteam();
            }
        }
        public string RunArguments { get; set; }
        public string Tab
        {
            get { return _tab; }
            set
            {
                _oldTab = _tab;
                _tab = value;
            }
        }
        public string OldTab { get { return _oldTab ?? _tab; } }
        public bool Installed { get; set; }
        public bool IsSteamApp { get; set; }
        public int SteamAppNumber { get; set; }
        public string Installer { get; set; }
        public string InstallArguments { get; set; }
        public string Uninstaller { get; set; }
        public string UninstallArguments { get; set; }
        #endregion

        void ConfigureSteam()
        {
            if (Configuration.SteamPath == FilePath)
                return;
            if (!File.Exists(FilePath))
                return;

            FileInfo file = new FileInfo(FilePath);
            if (String.Equals(file.Name, "steam.exe", StringComparison.OrdinalIgnoreCase))
            {
                Configuration.SteamPath = FilePath;
            }
        }

        public override string ToString()
        {
            if (IsSteamApp)
                return String.Format("Title: {0}, Steam App: {1}", Title, SteamAppNumber);           
            else
                return String.Format("Title: {0}, File Path: {1}", Title, FilePath);
        }

        public bool Equals(ApplicationModel other)
        {
            if (other == null)
                return false;

            // Application titles must be unique.
            if (String.Equals(Title, other.Title, StringComparison.InvariantCulture))
                return true;

            // Steam games need to be handled differently since the 'FilePath' property 
            // is the path to the steam executable.
            if (IsSteamApp || other.IsSteamApp)
            {
                // Both need to be steam games and they need to have the same application number.
                if (IsSteamApp == other.IsSteamApp && SteamAppNumber == other.SteamAppNumber)
                    return true;
                else
                    return false;
            }

            // Regular application
            if (String.Equals(FilePath, other.FilePath, StringComparison.InvariantCultureIgnoreCase) &&
                String.Equals(RunArguments, other.RunArguments, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;              
        }        
    }
}
