using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Microsoft.Win32;
using AstoundingApplications.AstoundingDock.Models;
using AstoundingApplications.AstoundingDock.Extensions;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AstoundingApplications.AstoundingDock.ViewModels
{   
    class SearchSteamViewModel : SearchViewModel
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        const string InstalledApps32Bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        const string InstalledApps64Bit = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        public SearchSteamViewModel(ObservableCollection<TabViewModel> tabs) : base(tabs)
        {
        }

        protected override void LoadApplications(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            foreach (string key in new[] { InstalledApps32Bit, InstalledApps64Bit })
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);

                if (registryKey == null)
                    continue; // The InstalledApps64Bit registry key won't exist on 32 bit machines

                for (int i = 0; i < registryKey.GetSubKeyNames().Length; i++)
                {
                    string subKey = registryKey.GetSubKeyNames()[i];

                    if (_loadWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    
                    // Only care about steam applications (i.e. "Steam App 10143")
                    Match match = Regex.Match(subKey, "^Steam App ([0-9]*)");
                    if (!match.Success)
                        continue;

                    RegistryKey propertyKey = registryKey.OpenSubKey(subKey);                    
                    if (propertyKey == null)
                        continue;                    
                    
                    string title = propertyKey.GetValue("DisplayName", null) as string;
                    string icon = propertyKey.GetValue("DisplayIcon", null) as string;

                    if (title.IsNullOrWhiteSpace())
                        continue;

                    try
                    {
                        int appNumber = Int32.Parse(match.Groups[1].Value);

                        ApplicationModel applicationModel = ApplicationModel.FromSteamApp(title, appNumber, icon, SelectedTab.Title);
                        ApplicationViewModel application = new ApplicationViewModel(applicationModel);

                        // Ignore applications already added to the dock
                        if (!Tabs.Any(obj => obj.Applications.Contains(application)))
                        {
                            AddApplication(application);
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        Log.DebugFormat("Failed to add steam application to search view - {0}", ex.Message);
                    }
                } // for
            } // foreach
        }      
    }
}
