using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using AstoundingApplications.AstoundingDock.Utils;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using AstoundingApplications.AstoundingDock.Services;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AppBarInterface;
using AstoundingApplications.AstoundingDock.Messages;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        ObservableCollection<string> _availableTabs;
        ObservableCollection<Screen> _screens;

        public SettingsViewModel()
        {
            _availableTabs = new ObservableCollection<string>();
            foreach (string tab in Configuration.AvailableTabs)
            {
                _availableTabs.Add(tab);
            }

            _screens = new ObservableCollection<Screen>();
            foreach (Screen screen in Screen.AllScreens)
            {
                _screens.Add(screen);
            }

            Themes = new ObservableCollection<string>();
            
            foreach (string theme in WpfHelper.AvailableThemes)
            {
                Themes.Add(theme);
            }
        }

        void ToggleRunOnStartup(bool value)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (value)
                registryKey.SetValue("AstoundingDock", FileHelper.GetApplicationExe());
            else
                registryKey.DeleteValue("AstoundingDock", false);   
        }

        public int IconRows
        {
            get { return Configuration.IconRows; }
            set { Configuration.IconRows = value; }
        }

        public DockEdge DockEdge
        {
            get { return Configuration.DockPosition.Selected; }
            set { Configuration.DockPosition = new DockPosition(value); }
        }

        public Screen ActiveScreen
        {
            get { return Configuration.ActiveScreen; }
            set { Configuration.ActiveScreen = value; }
        }

        public bool AutoHide
        {
            get { return Configuration.AutoHide; }
            set { Configuration.AutoHide = value; }
        }

        public int AutoHideDelay
        {
            get { return Configuration.AutoHideDelay; }
            set { Configuration.AutoHideDelay = value; }
        }

        public int PopupDelay
        {
            get { return Configuration.PopupDelay; }
            set { Configuration.PopupDelay = value; }
        }

        public bool ReserveScreen
        {
            get { return Configuration.ReserveScreen; }
            set { Configuration.ReserveScreen = value; }
        }

        public ApplicationFilter ApplicationFilter
        {
            get { return Configuration.ApplicationFilter; }
            set { Configuration.ApplicationFilter = value; }
        }

        public ObservableCollection<Screen> Screens
        {
            get { return _screens; }
        }

        public string DefaultTab
        {
            get { return Configuration.DefaultTab; }
            set { Configuration.DefaultTab = value; }
        }

        public bool RunOnStartup
        {
            get { return Configuration.RunOnStartup; }
            set
            {
                Configuration.RunOnStartup = value;
                ToggleRunOnStartup(value);         
            }
        }

        public ObservableCollection<string> AvailableTabs
        {
            get { return _availableTabs; }
        }

        public string Version
        {
            get  
            { 
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString(); 
            }
        }

        public ObservableCollection<string> Themes { get; private set; }
        public string CurrentTheme
        {
            get { return Configuration.CurrentTheme; }
            set
            {
                try
                {
                    WpfHelper.LoadTheme(value);
                    Configuration.CurrentTheme = value;

                    // After changing the theme it is nesscary to restart the application,
                    // otherwise the resources are not being correctly application to every
                    // element, due to the fact that it's not possible to use dynamic resources
                    // when using 'BasedOn'.

                    // TODO: For some reason this does not restart the application properly,
                    // from Visual Studio it will work three times in a row, from the installed
                    // application it does not work at all. 
                    // Seems to be some issue with the SingletonManager thing, issue with 
                    // timing or something???
                    System.Windows.Application.Current.MainWindow.Close();
                    System.Windows.Application.Current.Shutdown();
                    System.Diagnostics.Process.Start(Application.ExecutablePath);

                    /*
                    System.Windows.Application.Current.Exit += (sender, e) =>
                        {
                            System.Threading.Thread.Sleep(100);
                            System.Diagnostics.Process.Start(Application.ExecutablePath);
                        };
                    System.Windows.Application.Current.Shutdown();  
                     */

                    /*
                    System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

                    System.Windows.Application.Current.MainWindow.Closed += (sender, e) =>
                        {
                            System.Windows.Application.Current.Shutdown();                            
                        };
                    System.Windows.Application.Current.Exit += (sender, e) =>
                        {
                            System.Diagnostics.Process.Start(Application.ExecutablePath);
                        };

                    System.Windows.Application.Current.MainWindow.Close();   
                     */

                    /*
                    System.Windows.Application.Current.MainWindow.Closed += (sender, e) =>
                        {
                            System.Windows.Application.Current.Shutdown();
                            Application.Restart();                            
                        };
                    System.Windows.Application.Current.MainWindow.Close();     
                     */
                }
                catch (Exception ex)
                {
                    ServiceManager.GetService<IMessageBoxService>().Show(
                        String.Format("Failed to change theme {0}", ex.Message), "Astounding Dock");
                }
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                    });
            }
        }
    }
}
