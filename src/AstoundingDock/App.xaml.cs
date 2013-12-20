using System.Windows;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Markup;
using AstoundingApplications.AstoundingDock.ViewModels;
using AstoundingApplications.AstoundingDock.Services;
using AstoundingApplications.AstoundingDock.Views;
using AstoundingApplications.AstoundingDock.Models;
using AstoundingApplications.AstoundingDock.Utils;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Threading;
using AstoundingApplications.AstoundingDock.Messages;
using System.Windows.Media;
using System.Windows.Interop;
using System.Diagnostics;
using System;
using System.Collections;

namespace AstoundingApplications.AstoundingDock
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Debug("OnStartUp");

            InitializeComponent();
            base.OnStartup(e);                        

            Application.Current.DispatcherUnhandledException += OnUnhandledException;            

            // Load theme
            try
            {
                WpfHelper.LoadTheme(Configuration.CurrentTheme);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Failed to load theme {0}, {1}",
                    Configuration.CurrentTheme, ex.Message), "Astounding Dock",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Set default resources. Even if the theme fails to load, with these set the application should run fine.
            if (!this.Resources.Contains("WindowBackgroundBrush"))
                this.Resources.Add("WindowBackgroundBrush", new SolidColorBrush(Colors.Gray));
            if (!this.Resources.Contains("TextBrush"))
                this.Resources.Add("TextBrush", new SolidColorBrush(Colors.Black));

            // Register services
            RegisterServices();           

            // Load xml file
            XmlDatabase database = new XmlDatabase(Configuration.DatabaseFile);
            Application.Current.Exit += OnApplicationExit;

            // Opened the main window.       
            MainViewModel viewModel = new MainViewModel(database);
            ServiceManager.GetService<IViewService>().OpenWindow(viewModel);            
        }

        public void Activate()
        {
            Logger.Debug("Activate");

            var mainWindow = Application.Current.MainWindow as MainWindow;

            if (mainWindow == null)
            {
                MessageBox.Show("Unable to activate existing instance", "Astounding Dock");
                //Application.Current.Shutdown();
                return;
            }

            mainWindow.PopupWindow();
        }

        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            Logger.Debug("OnApplicationExit");

            // When the application closes, explictly save the database to avoid
            // any information being lost due to the wait-timer on the save.
            Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Save());
        }

        void RegisterServices()
        {
            IViewService viewService = ServiceManager.RegisterService<IViewService>(new ViewService());
            viewService.RegisterView(typeof(MainWindow), typeof(MainViewModel));
            viewService.RegisterView(typeof(AboutWindow), typeof(AboutViewModel));
            viewService.RegisterView(typeof(SettingsWindow), typeof(SettingsViewModel));
            viewService.RegisterView(typeof(ApplicationWindow), typeof(ApplicationViewModel));
            viewService.RegisterView(typeof(TabWindow), typeof(TabViewModel));
            viewService.RegisterView(typeof(MessageBoxWindow), typeof(MessageBoxViewModel));
            viewService.RegisterView(typeof(SearchWindow), typeof(SearchNewViewModel));
            viewService.RegisterView(typeof(SearchDockWindow), typeof(SearchDockViewModel));
            viewService.RegisterView(typeof(SearchSteamWindow), typeof(SearchSteamViewModel));

            ServiceManager.RegisterService<IOpenFileService>(new OpenFileService());
            ServiceManager.RegisterService<IMessageBoxService>(new MessageBoxService());
        }

        void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                var result = ServiceManager.GetService<IMessageBoxService>().Show(e.Exception.Message,
                    "Astounding Dock -- Unexpected ErrorEvent", MessageIcon.Error, MessageOptions.ContinueClose);

                if (result == MessageResult.Close)
                {
                    Application.Current.Shutdown();
                }
            }
            catch
            {
                // If there is a problem with the ui message service.
                MessageBox.Show(e.Exception.Message, "Astounding Dock -- ErrorEvent",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            e.Handled = true;
        }
    }
}
