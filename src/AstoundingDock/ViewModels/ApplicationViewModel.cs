using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Models;
using AstoundingApplications.AstoundingDock.Services;
using AstoundingApplications.AstoundingDock.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class ApplicationViewModel : ViewModelBase, IEquatable<ApplicationViewModel>, IEquatable<ApplicationModel>
    {       
        public ApplicationViewModel() : this (new ApplicationModel()) {}

        public ApplicationViewModel(ApplicationModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            
            Model = model;
            Installed = IsAppInstalled();
        }

        #region Properties
        public MessageResult Result { get; private set; }
        public ApplicationModel Model { get; private set; }
        public List<string> AvailableTabs { get { return Configuration.AvailableTabs; } }
        public string Title
        {
            get { return Model.Title; }
            set { Model.Title = value; }
        }
        public string OldTitle { get { return Model.OldTitle; } }
        public string FilePath
        {
            get { return Model.FilePath; }
            set { Model.FilePath = value; }
        }
        public string RunArguments
        {
            get { return Model.RunArguments; }
            set { Model.RunArguments = value; }
        }
        public string ImagePath
        {
            get
            {
                if (File.Exists(Model.ImagePath))
                    return Model.ImagePath;
                return Model.FilePath;
            }
            set
            {
                Model.ImagePath = value;
            }
        }
        public string Tab
        {
            get { return Model.Tab; }
            set { Model.Tab = value; }
        }
        public string OldTab { get { return Model.OldTab; } }
        public bool Installed
        {
            get { return Model.Installed; }
            set { Model.Installed = value; }
        }
        public string Installer
        {
            get { return Model.Installer; }
            set { Model.Installer = value; }
        }
        public string InstallArguments
        {
            get { return Model.InstallArguments; }
            set { Model.InstallArguments = value; }
        }
        public string Uninstaller
        {
            get { return Model.Uninstaller; }
            set { Model.Uninstaller = value; }
        }
        public string UninstallArguments
        {
            get { return Model.UninstallArguments; }
            set { Model.UninstallArguments = value; }
        }
        public int SteamAppNumber
        {
            get { return Model.SteamAppNumber; }
            set { Model.SteamAppNumber = value; }
        }
        public bool IsSteamApp
        {
            get { return Model.IsSteamApp; }
            set { Model.IsSteamApp = value; }
        }
        #endregion

        public override string ToString()
        {
            return Model.ToString();
        }

        public bool Equals(ApplicationViewModel other)
        {
            if (other == null)
                return false;
            return Model.Equals(other.Model);
        }

        public bool Equals(ApplicationModel other)
        {
            return Model.Equals(other);
        }

        public void UpdateWith(ApplicationViewModel other)
        {
            Title = other.Title;
            FilePath = other.FilePath;
            RunArguments = other.RunArguments;
            ImagePath = other.ImagePath;
            Tab = other.Tab;
            Installed = other.Installed;
            Installer = other.Installer;
            InstallArguments = other.InstallArguments;
            Uninstaller = other.Uninstaller;
            UninstallArguments = other.UninstallArguments;
            IsSteamApp = other.IsSteamApp;
            SteamAppNumber = other.SteamAppNumber;
        }

        /// <summary>
        /// Determines if the application is 'actually' installed, regardless of what
        /// value the 'Installed' flag has been set to in the database.
        /// </summary>
        bool IsAppInstalled()
        {
            if (IsSteamApp)            
                return IsSteamAppInstalled();            
            else            
                return File.Exists(FilePath);            
        }

        /// <summary>
        /// Determines if the steam application is installed or not by checking 
        /// if it'str in the registry.
        /// </summary>
        bool IsSteamAppInstalled()
        {
            foreach (var registryKey in RegistryHelper.GetInstallApplications())
            {
                Match match = Regex.Match(registryKey, String.Format("^Steam App {0}", SteamAppNumber));
                if (match.Success)
                {
                    return true;
                }
            }
            return false;            
        }

        bool Validate()
        {
            if (String.IsNullOrWhiteSpace(Title))
            {
                ServiceManager.GetService<IMessageBoxService>().Show(
                    "Title not set", "Astounding Dock");
                return false;
            }

            if (!File.Exists(FilePath))
            {
                ServiceManager.GetService<IMessageBoxService>().Show(
                    "Invalid file path", "Astounding Dock");
                return false;
            }

            return true;
        }

        void RunInstaller(string installer, bool isTemporary)
        {
            Process process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    FileName = installer,
                    Arguments = InstallArguments,
                    WorkingDirectory = Path.GetDirectoryName(installer)
                }
            };

            Observable.FromEventPattern(process, "Exited").Take(1).Subscribe(ep =>
            {
                process.Dispose();

                if (isTemporary)
                {
                    try
                    {
                        File.Delete(installer);
                    }
                    catch (IOException)
                    {
                        // Don't care if this fails.
                    }
                }
            });            

            try
            {
                process.Start();
            }
            catch (Win32Exception ex)
            {
                ServiceManager.GetService<IMessageBoxService>().Show(String.Format(
                    "Failed to install application {0}", ex.Message), "Astounding Dock", MessageIcon.Error);
                process.Dispose();
            }
        }

        #region Commands
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!Validate())                    
                        return;
                    
                    Result = MessageResult.Okay;
                    Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                });
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Result = MessageResult.Cancel;
                    Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                });
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.Edit(this));
                });
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.Remove(this));
                });
            }
        }

        public ICommand RunCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (!File.Exists(FilePath))
                    {
                        ServiceManager.GetService<IMessageBoxService>().Show("Invalid file path", "Astounding Dock", MessageIcon.Error);
                        return;
                    }

                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = FilePath;
                        process.StartInfo.Arguments = RunArguments;
                        process.StartInfo.WorkingDirectory = Path.GetDirectoryName(FilePath);

                        try
                        {
                            process.Start();
                        }
                        catch (Win32Exception ex)
                        {
                            ServiceManager.GetService<IMessageBoxService>().Show(
                                "Failed to start application - " + ex.Message, "Astounding Dock", MessageIcon.Error);
                        }
                    }
                });
            }
        }

        public ICommand UpdatePathCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var openFileService = ServiceManager.GetService<IOpenFileService>();
                    openFileService.Filter = String.Format("{0}|{1}", FileFilters.ExecutableFiles, FileFilters.AllFiles);

                    if (File.Exists(FilePath))
                        openFileService.InitialDirectory = Path.GetDirectoryName(FilePath);
                    else
                        openFileService.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                    bool? success = openFileService.ShowDialog();
                    if (success == true)
                    {
                        FilePath = openFileService.OpenedFile;
                        if (String.IsNullOrWhiteSpace(Title))
                            Title = Path.GetFileNameWithoutExtension(FilePath);

                        RaisePropertyChanged("ImagePath");
                    }
                });
            }
        }

        public ICommand UpdateImageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var openFileService = ServiceManager.GetService<IOpenFileService>();
                    openFileService.Filter = String.Format("{0}|{1}|{2}", FileFilters.ImageFiles, FileFilters.ExecutableFiles, FileFilters.AllFiles);

                    if (IsSteamApp && String.Equals(ImagePath, Configuration.SteamPath, StringComparison.InvariantCultureIgnoreCase))
                        openFileService.InitialDirectory = FileHelper.GetSteamIconsDirectory(Configuration.SteamPath);
                    else if (File.Exists(ImagePath)) // This is the path of the image, or if no image is set the then path of the exe
                        openFileService.InitialDirectory = Path.GetDirectoryName(ImagePath);
                    else
                        openFileService.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                    bool? result = openFileService.ShowDialog();
                    if (result == true)
                        ImagePath = openFileService.OpenedFile;                    
                });
            }
        }

        public ICommand ResetImageCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ImagePath = FilePath;
                });
            }
        }

        public ICommand UpdateInstallerCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    // The directory which initially opens should be the directory the existing installer is in,
                    // if the uninstaller was set previously.      
                    var openFileService = ServiceManager.GetService<IOpenFileService>();
                    openFileService.Filter = String.Format("{0}|{1}", FileFilters.ExecutableFiles, FileFilters.AllFiles);

                    if (File.Exists(Installer))
                        openFileService.InitialDirectory = Path.GetDirectoryName(Installer);
                    else if (File.Exists(FilePath))
                        openFileService.InitialDirectory = Path.GetDirectoryName(FilePath);
                    else
                        openFileService.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                    bool? result = openFileService.ShowDialog();
                    if (result == true)
                        Installer = openFileService.OpenedFile;
                });
            }
        }

        public ICommand UpdateUninstallerCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    // The directory which initially opens should be the directory the existing uninstaller is in,
                    // if the uninstaller was set previously.                  
                    var openFileService = ServiceManager.GetService<IOpenFileService>();
                    openFileService.Filter = String.Format("{0}|{1}", FileFilters.ExecutableFiles, FileFilters.AllFiles);

                    if (File.Exists(Uninstaller))
                        openFileService.InitialDirectory = Path.GetDirectoryName(Uninstaller);
                    else if (File.Exists(FilePath))
                        openFileService.InitialDirectory = Path.GetDirectoryName(FilePath);
                    else
                        openFileService.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

                    bool? result = openFileService.ShowDialog();
                    if (result == true)
                        Uninstaller = openFileService.OpenedFile;
                });
            }
        }

        public ICommand InstallCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (String.IsNullOrWhiteSpace(FilePath))
                        return;

                    if (File.Exists(Installer))
                    {
                        // Install from file.
                        RunInstaller(Installer, false);
                    }
                    else if (Uri.IsWellFormedUriString(Installer, UriKind.Absolute))
                    {
                        // Install from url. 

                        // 1) Download from URL.
                        // 2) Copy to temporary file.
                        // 3) Run temporary file.
                        // 4) Remove Temporary file.

                        Uri uri = new Uri(Installer);
                        string tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(Installer));

                        var messageBoxService = ServiceManager.GetService<IMessageBoxService>();
                        WebClient webClient = new WebClient();

                        Observable.FromEventPattern<AsyncCompletedEventArgs>(webClient, "DownloadFileCompleted").Take(1).Subscribe(ep =>
                            {
                                messageBoxService.CloseLast();
                                webClient.Dispose();

                                if (ep.EventArgs.Cancelled)
                                    return;

                                if (ep.EventArgs.Error != null)
                                {
                                    messageBoxService.Show(String.Format(
                                        "Unable to download file, recieved the following error\n    {0}", ep.EventArgs.Error),
                                        "Astounding Dock", MessageIcon.Error);
                                    return;
                                }

                                RunInstaller(tempFile, true);
                            }); // DownloadFileCompleted

                        // Popup a message box allowing the user to cancel the download attempt.
                        MessageResult result = messageBoxService.Show(String.Format(
                            "Attempting to download file from url {0}", uri), "Astounding Dock",
                            MessageIcon.Information, MessageOptions.Cancel);

                        if (result == MessageResult.Cancel)
                            webClient.CancelAsync();

                    } // Uri.IsWellFormedUriString
                    else
                    {
                        ServiceManager.GetService<IMessageBoxService>().Show("Invalid installer", "Astounding Dock", MessageIcon.Error);
                    }
                });
            }
        }

        public ICommand UninstallCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (String.IsNullOrWhiteSpace(Uninstaller))
                        return;

                    if (!File.Exists(Uninstaller))
                    {
                        ServiceManager.GetService<IMessageBoxService>().Show("Invalid uninstaller", "Astounding Dock", MessageIcon.Error);
                        return;
                    }

                    Process process = new Process()
                    {
                        EnableRaisingEvents = true,
                        StartInfo =
                        {
                            FileName = Uninstaller,
                            Arguments = UninstallArguments,
                            WorkingDirectory = Path.GetDirectoryName(Uninstaller)
                        }
                    };

                    Observable.FromEventPattern(process, "Exited").Take(1).Subscribe(ep =>
                    {
                        if (process.ExitCode == 0 && !File.Exists(FilePath))
                            Installed = false;
                        process.Dispose();
                    });

                    try
                    {
                        process.Start();
                    }
                    catch (Win32Exception ex)
                    {
                        ServiceManager.GetService<IMessageBoxService>().Show(String.Format(
                            "Failed to uninstall application - {0}", ex.Message), "Astounding Dock", MessageIcon.Error); ;
                        process.Dispose();
                    }
                });
            }
        }  

        #endregion
    }
}
