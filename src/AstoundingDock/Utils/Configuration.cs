using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IniParser;
using System.IO;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AstoundingDock.Messages;
using System.Windows.Forms;
using AstoundingApplications.AppBarInterface;
using System.Diagnostics.Contracts;
using AstoundingApplications.AstoundingDock.Extensions;

namespace AstoundingApplications.AstoundingDock.Utils
{
    [TypeConverter(typeof(EnumDescriptionConverter))]
    enum ApplicationFilter 
    {         
        [Description("All")] 
        All, 
        [Description("Installed Only")]
        InstalledOnly, 
        [Description("Uninstalled Only")]
        UninstalledOnly 
    }

    class Configuration
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const int IconRowsDefault = 3;
        public const string DefaultTheme = "OdysseyBlue";

        static readonly Configuration _instance = new Configuration();
        readonly FileIniDataParser _parser = new FileIniDataParser();
        readonly List<string> availableTabs;
        IniData _data;  
      
        Configuration()
        {
            string settingsFile = Path.Combine(FileHelper.GetSettingsDirectory(), "Settings.ini");
            _parser = new FileIniDataParser();
            availableTabs = new List<string>();

            if (File.Exists(settingsFile))
            {
                _data = _parser.LoadFile(settingsFile);
            }
            else
            {
                _data = CreateDefault(settingsFile);
                Save();
            }
        }

        public static List<string> AvailableTabs
        {
            get { return _instance.availableTabs; }
        }

        public static string DefaultTab
        {
            get { return _instance.Get("DefaultTab", "General"); }
            set { _instance.Set("DefaultTab", value); }
        }

        public static string DatabaseFile
        {
            get { return _instance.Get("DatabaseFile"); }
            set { _instance.Set("DatabaseFile", value); }
        }

        public static string SteamPath
        {
            get { return _instance.Get("SteamPath") ?? ""; }
            set { _instance.Set("SteamPath", value); }
        }

        public static int IconRows
        {
            get { return _instance.GetInt("IconRows", 3); }
            set { _instance.Set("IconRows", value); }
        }

        public static DockPosition DockPosition
        {
            get { return _instance.GetDockPosition("DockPosition"); }
            set { _instance.Set("DockPosition", value); }
        }

        public static Screen ActiveScreen
        {
            get
            {
                string value = _instance.Get("ActiveScreen");
                return Helper.GetScreenFromName(value);
            }
            set
            {
                _instance.Set("ActiveScreen", value.DeviceName.CleanString());
            }
        }

        public static bool AutoHide
        {
            get { return _instance.GetBool("AutoHide", true); }
            set { _instance.Set("AutoHide", value); }
        }

        public static int AutoHideDelay
        {
            get { return _instance.GetInt("AutoHideDelay", 1000); }
            set { _instance.Set("AutoHideDelay", value); }
        }

        public static int PopupDelay
        {
            get { return _instance.GetInt("PopupDelay", 1000); }
            set { _instance.Set("PopupDelay", value); }
        }

        public static bool ReserveScreen
        {
            get { return _instance.GetBool("ReserveScreen", false); }
            set { _instance.Set("ReserveScreen", value); }
        }

        public static ApplicationFilter ApplicationFilter
        {
            get { return _instance.GetEnum<ApplicationFilter>("ApplicationFilter"); }
            set { _instance.Set("ApplicationFilter", value); }
        }

        public string DefaultSection
        {
            get { return _instance.Get("DefaultSection", "General"); }
            set { _instance.Set("DefaultSection", value); }
        }

        public static string CurrentTheme
        {
            get { return _instance.Get("CurrentTheme", DefaultTheme); }
            set { _instance.Set("CurrentTheme", value); }
        }

        public static bool RunOnStartup
        {
            get { return _instance.GetBool("ToggleRunOnStartup", false); }
            set { _instance.Set("ToggleRunOnStartup", value); }
        }

        DockPosition GetDockPosition(string key)
        {
            string value = Get(key);
            if (Enum.IsDefined(typeof(DockEdge), value))
                return new DockPosition((DockEdge)Enum.Parse(typeof(DockEdge), value, true));
            return new DockPosition(DockEdge.Right);
        }

        T GetEnum<T>(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            string value = Get(key);
            if (Enum.IsDefined(typeof(T), value))
                return (T)Enum.Parse(typeof(T), value, true);
            return default(T);
        }

        int GetInt(string key, int defaultValue)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            int value;
            if (Int32.TryParse(Get(key), out value))
                return value;
            return defaultValue;
        }

        bool GetBool(string key, bool defaultValue)
        {
            bool value;
            if (Boolean.TryParse(Get(key), out value))
                return value;
            return defaultValue;
        }

        string Get(string key)
        {
            return Get(key, "");
        }

        string Get(string key, string defaultValue)
        {
            if (_data == null)
                throw new ArgumentNullException("_data");
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key");;

            if (!_data.Sections.ContainsSection("General"))
                _data.Sections.AddSection("General");

            if (_data.Sections["General"].ContainsKey(key))
                return _data.Sections["General"][key];
            return defaultValue;
        }

        void Set(string key, object value)
        {
            if (_data == null)
                throw new ArgumentNullException("_data");
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            if (!_data.Sections.ContainsSection("General"))
                _data.Sections.AddSection("General");

            if (_data.Sections["General"].ContainsKey(key))
                _data.Sections["General"][key] = value.ToString();
            else
                _data.Sections["General"].AddKey(key, value.ToString());
            Save();

            Messenger.Default.Send<SettingChangedMessage>(new SettingChangedMessage(key, value));            
        }

        IniData CreateDefault(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path");

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var data = new IniData();
            data.Sections.AddSection("General");

            data.Sections["General"].AddKey("DatabaseFile", Path.Combine(FileHelper.GetSettingsDirectory(), "Applications.xml"));
            data.Sections["General"].AddKey("SteamPath", "");
            data.Sections["General"].AddKey("DefaultTab", "General");
            data.Sections["General"].AddKey("IconRows", "3");
            data.Sections["General"].AddKey("ActiveScreen", Screen.PrimaryScreen.DeviceName.CleanString());
            data.Sections["General"].AddKey("DockPosition", DockEdge.Right.ToString());
            data.Sections["General"].AddKey("AutoHide", Boolean.TrueString);
            data.Sections["General"].AddKey("AutoHideDelay", "1000");
            data.Sections["General"].AddKey("PopupDelay", "1000");
            data.Sections["General"].AddKey("ReserveScreen", Boolean.TrueString);
            data.Sections["General"].AddKey("ApplicationFilter", ApplicationFilter.All.ToString());
            data.Sections["General"].AddKey("CurrentTheme", DefaultTheme);
            data.Sections["General"].AddKey("ToggleRunOnStartup", Boolean.FalseString);
   
            return data;
        }

        void Save()
        {
            if (_parser == null)
                throw new ArgumentNullException("_parser");
            if (_data == null)
                throw new ArgumentNullException("_data");

            _parser.SaveFile(Path.Combine(FileHelper.GetSettingsDirectory(), "Settings.ini"), _data);
        }
    }
}
