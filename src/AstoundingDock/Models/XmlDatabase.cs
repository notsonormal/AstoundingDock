using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.IO;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Models;
using AstoundingApplications.AstoundingDock.Extensions;
using System.Xml.Linq;
using System.Timers;

namespace AstoundingApplications.AstoundingDock.Models
{
    class XmlDatabase
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string _fileName;
        XElement _database;
        Timer _saveTimer;

        public XmlDatabase(string databaseLocation)
        {
            if (String.IsNullOrWhiteSpace(databaseLocation))
                throw new ArgumentException("databaseLocation");

            _fileName = databaseLocation;

            if (!File.Exists(databaseLocation))
                CreateDatabase();

            _database = XElement.Load(databaseLocation);            

            _saveTimer = new Timer();
            _saveTimer.AutoReset = false;
            _saveTimer.Elapsed += (sender, e) => DoSave();
            _saveTimer.Interval = 5000;

            Messenger.Default.Register<DatabaseMessage>(this, OnDatabaseMessage);
        }

        public IEnumerable<TabModel> LoadTabs()
        {            
            var xml = from elem in _database.Element("Tabs").Elements("Tab") 
                      select elem;

            foreach (XElement xmlSection in xml)
            {
                TabModel tabModel = TabFromXml(xmlSection);
                foreach (var appModel in LoadApplications(tabModel))
                {
                    tabModel.Applications.Add(appModel);
                }
                yield return tabModel;
            }
        }

        public IEnumerable<ApplicationModel> LoadApplications(TabModel forThis)
        {
            if (forThis == null)
                throw new ArgumentNullException("forThis");

            return from elem in _database.Element("Applications").Elements("Application")
                   where elem.GetValue("Tab") == forThis.Title 
                   select ApplicationFromXml(elem);
        }

        void Save()
        {
            // Limit the number of times which we try and save the xml file 
            // in a period of time, otherwise we might run into errors.
            if (!_saveTimer.Enabled)
            {
                _saveTimer.Start();
            }
        }

        void DoSave()
        {
            Log.DebugFormat("DoSave");

            try
            {
                // Sometimes there is a XML error and when the save happens it creates an
                // empty file, if not happens you shouldn't save
                if (_database.HasElements)
                {
                    File.Copy(_fileName, String.Format("{0}.bak", _fileName), true); // Backup first
                    _database.Save(_fileName);
                }
                else
                {
                    Log.WarnFormat("Attempted to save but for some reason there were no xml elements, save aborted");
                }
            }
            catch (IOException ex)
            {
                Log.WarnFormat("Save failed {0}", ex);
            }
        }

        void OnDatabaseMessage(DatabaseMessage message)
        {
            try
            {
                switch (message.Action)
                {
                    case DatabaseMessage.ActionType.Add:
                        if (message.Application != null)
                            Add(message.Application);
                        else if (message.Tab != null)
                            Add(message.Tab);
                        break;
                    case DatabaseMessage.ActionType.Remove:
                        if (message.Application != null)
                            Remove(message.Application);
                        else if (message.Tab != null)
                            Remove(message.Tab);
                        break;
                    case DatabaseMessage.ActionType.Update:
                        if (message.Application != null)
                            Update(message.Application);
                        else if (message.Tab != null)
                            Update(message.Tab);
                        break;
                    case DatabaseMessage.ActionType.Move:
                        if (message.Tab != null && message.TabB != null)
                            Move(message.Tab, message.TabB);
                        break;
                    case DatabaseMessage.ActionType.Save:
                        DoSave();
                        break;
                }

                // Save changes.
                Save();
            }
            catch (Exception ex)
            {
                Log.FatalFormat("Database exception {0}", ex);
            }
        }

        void Add(ApplicationModel addThis)
        {
            Log.DebugFormat("Add {0}", addThis.Title);

            _database.Element("Applications").Add(ApplicationToXml(addThis));
        }

        void Remove(ApplicationModel removeThis)
        {
            Log.DebugFormat("Remove {0}", removeThis.Title);

            var application = GetApplication(removeThis);
            application.Remove();
        }

        void Update(ApplicationModel updateThis)
        {
            Log.DebugFormat("Update {0}", updateThis.Title);

            var application = GetApplication(updateThis);
            application.ReplaceWith(ApplicationToXml(updateThis));
        }

        void Add(TabModel addThis)
        {
            Log.DebugFormat("Add {0}", addThis.Title);

            _database.Element("Tabs").Add(TabToXml(addThis));
        }

        void Remove(TabModel removeThis)
        {
            Log.DebugFormat("Remove {0}", removeThis.Title);

            var section = GetTab(removeThis);
            section.Remove();
        }

        void Update(TabModel updateThis)
        {
            Log.DebugFormat("Update {0}", updateThis.OldTitle);

            var tab = GetTab(updateThis);
            tab.ReplaceWith(TabToXml(updateThis));
        }

        void Move(TabModel moveThis, TabModel moveToThis)
        {
            Log.DebugFormat("Move {0} to {1}", moveThis.Title, moveToThis.Title);

            var moveThisXml = GetTab(moveThis);
            var moveToThisXml = GetTab(moveToThis);

            // Move xml from current position to just above the other tab.
            moveThisXml.Remove();
            moveToThisXml.AddBeforeSelf(moveThisXml);
        }

        XElement GetTab(TabModel tab)
        {
           return
                (from sectionXml in _database.Element("Tabs").Elements("Tab")
                 where sectionXml.Element("Title").Value == tab.Title ||
                       sectionXml.Element("Title").Value == tab.OldTitle
                 select sectionXml).Single();
        }

        XElement GetApplication(ApplicationModel application)
        {
            return
                (from applicationXml in _database.Element("Applications").Elements("Application")
                 where applicationXml.Element("Title").Value == application.Title ||
                       applicationXml.Element("Title").Value == application.OldTitle
                 select applicationXml).Single();
        }

        XElement TabToXml(TabModel section)
        {
            XElement xml = new XElement("Tab",
                    new XElement("Title", section.Title),
                    new XElement("IsExpanded", section.IsExpanded),
                    new XElement("TabOrder", section.TabOrder));
            return xml;
        }

        TabModel TabFromXml(XElement xml)
        {
            string title = xml.GetValue("Title");
            bool isExpanded = xml.GetValue<bool>("IsExpanded", false);
            int tabOrder = xml.GetValue<int>("TabOrder", 1);

            return new TabModel(title, isExpanded, tabOrder);
        }

        XElement ApplicationToXml(ApplicationModel application)
        {
            XElement xml = new XElement("Application",
                    new XElement("Title", application.Title),
                    new XElement("FilePath", application.FilePath),
                    new XElement("RunArguments", application.RunArguments),
                    new XElement("Tab", application.Tab),
                    new XElement("ImagePath", application.ImagePath),
                    new XElement("IsSteamApp", application.IsSteamApp),
                    new XElement("SteamAppNumber", application.SteamAppNumber),
                    new XElement("Installed", application.Installed),
                    new XElement("Installer", application.Installer),
                    new XElement("InstallArguments", application.InstallArguments),
                    new XElement("Uninstaller", application.Uninstaller),
                    new XElement("UninstallArguments", application.UninstallArguments));

            return xml;
        }

        ApplicationModel ApplicationFromXml(XElement xml)
        {
            ApplicationModel application = new ApplicationModel()
            {
                Title = xml.GetValue("Title"),
                FilePath = xml.GetValue("FilePath"),
                RunArguments = xml.GetValue("RunArguments"),
                Tab = xml.GetValue("Tab"),
                ImagePath = xml.GetValue("ImagePath"),
                IsSteamApp = xml.GetValue<bool>("IsSteamApp", false),
                SteamAppNumber = xml.GetValue<int>("SteamAppNumber", 0),
                Installed = xml.GetValue<bool>("Installed", false),
                Installer = xml.GetValue("Installer"),
                InstallArguments = xml.GetValue("InstallArguments"),
                Uninstaller = xml.GetValue("Uninstaller"),
                UninstallArguments = xml.GetValue("UninstallArguments")
            };

            return application;
        }

        void CreateDatabase()
        {
            if (String.IsNullOrWhiteSpace(_fileName))
                throw new ArgumentException("_fileName");

            string directory = Path.GetDirectoryName(_fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            XDocument document =
                new XDocument(
                    new XDeclaration("1.0", Encoding.UTF8.HeaderName, String.Empty),
                    new XElement("root",
                        new XElement("Tabs",
                            new XElement("Tab",
                                new XElement("Title", "General"),
                                new XElement("IsExpanded", "True"),
                                new XElement("TabOrder", "1")
                            )
                        ),
                        new XElement("Applications")));

            document.Save(_fileName);
        }
    }
}
