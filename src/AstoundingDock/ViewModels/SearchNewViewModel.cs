using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using AstoundingApplications.AstoundingDock.Extensions;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Models;
using AstoundingApplications.AstoundingDock.Utils;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using VistaBridge.Shell;
using AstoundingApplications.AstoundingDock.Services;

using PathEx = AstoundingApplications.AstoundingDock.Extensions.PathEx;
using GalaSoft.MvvmLight.Threading;
using System.Threading;
using System.Windows.Threading;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class SearchNewViewModel : SearchViewModel
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SearchNewViewModel(ObservableCollection<TabViewModel> tabs) : base(tabs)
        {
        }

        protected override void LoadApplications(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            foreach (string fileName in Directory.EnumerateFileSystemEntries(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "*.*", SearchOption.AllDirectories))
            {
                if (_loadWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                DispatcherHelper.UIDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        ProcessFile(fileName);
                    }));
            }
        }

        void ProcessFile(string fileName)
        {
            if (Log.IsDebugEnabled)
                Log.DebugFormat("Trying to load {0}", fileName);

            if (PathEx.HasExtension(fileName, FileExtensions.Executable) ||
                PathEx.HasExtension(fileName, FileExtensions.Shortcut) ||
                PathEx.HasExtension(fileName, FileExtensions.ClickOnce))
            {
                try
                {
                    ApplicationModel model = ApplicationModel.FromFile(fileName, SelectedTab.Title, true);

                    if (model == null)
                        return;

                    if (model.FilePath.StartsWith(@"C:\Windows", StringComparison.InvariantCultureIgnoreCase))
                        return; // Don't add anything from the windows directory                

                    if (!PathEx.HasExtension(model.FilePath, FileExtensions.Executable))
                        return; // Only take executable files

                    if (model.Title.Contains("Install", StringComparison.InvariantCultureIgnoreCase))
                        return; // Ignore installers   

                    if (model.Title.Contains("Uninstall", StringComparison.InvariantCultureIgnoreCase))
                        return; // Ignore uninstallers                

                    ApplicationViewModel application = new ApplicationViewModel(model);
                    if (Tabs.Any(obj => obj.Applications.Contains(application)))
                        return; // Ignore applications already added to the dock

                    AddApplication(new ApplicationViewModel(model));
                }
                catch (Exception ex)
                {
                    if (Log.IsDebugEnabled)
                        Log.DebugFormat("Failed to add file to search view - {0}", ex.Message);
                }
            }
        }

        public ICommand AddManualCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    // TODO: This doesn't work properly, it should try and add a new application, a dialog window needs to popup.
                    Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.AddNew());
                });
            }
        }

        public ICommand SearchSteamCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ServiceManager.GetService<IViewService>().OpenDialog(new SearchSteamViewModel(Tabs));
                });
            }
        }

    }
}
