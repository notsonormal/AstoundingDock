using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using AstoundingApplications.AstoundingDock.Services;
using System.Reflection;
using AstoundingApplications.AstoundingDock.Messages;
using System.Diagnostics;
using System.ComponentModel;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public string Version { get; set; }

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

        public ICommand NavigateCommand
        {
            get
            {
                return new RelayCommand<string>((url) =>
                    {
                        try
                        {
                            Process.Start(url);
                        }
                        catch (Win32Exception ex)
                        {
                            // System.ComponentModel.Win32Exception is a known exception that occurs when 
                            // Firefox is default browser, even when it correctly opens the page.
                            if (ex.ErrorCode == -2147467259)
                            {
                                // Special case, no browser error code.
                                ServiceManager.GetService<IMessageBoxService>().Show(ex.Message, "Astounding Dock");
                            }
                        }
                        catch (Exception)
                        {
                            try
                            {
                                // Got exception, try and open in internet explorer.
                                Process.Start("IExplore.exe", url);
                            }
                            catch (Exception ex)
                            {
                                // Still failed to open the page.
                                ServiceManager.GetService<IMessageBoxService>().Show(ex.Message, "Astounding Dock");
                            }
                        }
                    });
            }
        }
    }
}
