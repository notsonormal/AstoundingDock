using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using AstoundingApplications.AstoundingDock.Extensions;

namespace AstoundingApplications.AstoundingDock.Utils
{
    class RegistryHelper
    {
        const string installedApps32Bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        const string installedApps64Bit = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        static DateTime LastChecked;
        static IEnumerable<string> Installed = null;

        static IEnumerable<string> GetInstalledApplications()
        {
            List<string> installed = new List<string>();

            List<RegistryKey> keys = new List<RegistryKey>()
            {
                Registry.LocalMachine.OpenSubKey(installedApps32Bit),
                Registry.LocalMachine.OpenSubKey(installedApps64Bit)
            };

            foreach (RegistryKey registryKey in keys)
            {
                // The installedApps64Bit registry key won't exist on 32-bit machines
                if (registryKey != null)
                {
                    for (int i = 0; i < registryKey.GetSubKeyNames().Length; i++)
                    {
                        installed.Add(registryKey.GetSubKeyNames()[i]);
                    }
                }
            }
            return installed;        
        }

        public static IEnumerable<string> GetInstallApplications()
        {
            // If the registy was queried in the last 5 mins, just use the cached list. 
            // This avoides the entire list of installed applications being checked every time.
            if (Installed == null)
            {
                Installed = GetInstalledApplications();                
            }
            else
            {
                if ((LastChecked - DateTime.Now).TotalMinutes > 5)
                {
                    Installed = GetInstalledApplications();
                }
            }
            
            LastChecked = DateTime.Now;
            return Installed;
        }

        public static string GetSteamDisplayIcon(string steamAppNumber)
        {
            List<RegistryKey> keys = new List<RegistryKey>()
            {
                Registry.LocalMachine.OpenSubKey(installedApps32Bit),
                Registry.LocalMachine.OpenSubKey(installedApps64Bit)
            };

            foreach (RegistryKey registryKey in keys)
            {
                // The installedApps64Bit registry key won't exist on 32-bit machines
                if (registryKey != null)
                {
                    var steamAppKey = registryKey.OpenSubKey("Steam App " + steamAppNumber);
                    if (steamAppKey != null)
                    {
                        return steamAppKey.GetValue("DisplayIcon").IfNotNull(v => v.ToString());
                    }
                }
            }

            return null;
        }
    }
}
