using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using AstoundingApplications.AstoundingDock.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AstoundingDock.Messages;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics.Contracts;
using AstoundingApplications.AstoundingDock.Services;
using System.Windows.Data;
using AstoundingApplications.AstoundingDock.Utils;
using System.ComponentModel;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class TabViewModel : ViewModelBase, IEquatable<TabViewModel>
    {
        static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TabViewModel() : this(new TabModel()) { }

        public TabViewModel(TabModel model)
        {
            Model = model;                       

            Applications = new ObservableCollection<ApplicationViewModel>();
            foreach (var application in model.Applications)
            {
                Applications.Add(new ApplicationViewModel(application));
            }
            Applications.CollectionChanged += OnApplicationsChanged;

            ApplicationsView = (ListCollectionView)CollectionViewSource.GetDefaultView(Applications);
            ApplicationsView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
            UpdateFilter(Configuration.ApplicationFilter);

            Messenger.Default.Register<SettingChangedMessage>(this, (message) =>
                {
                    switch (message.Name)
                    {
                        case "ApplicationFilter":
                            UpdateFilter((ApplicationFilter)message.Value);
                            break;
                    }
                });
        }

        #region Properties
        public TabModel Model { get; private set; }
        public MessageResult Result { get; private set; }
        public string Title
        {
            get { return Model.Title; }
            set { Model.Title = value; }
        }
        public bool IsExpanded
        {
            get { return Model.IsExpanded; }
            set 
            { 
                Model.IsExpanded = value;

                /*
                if (value)
                {
                    // Only one tabModel should be expanded at any one time.
                    Messenger.Default.Send<ExpandedTabMessage>(
                        new ExpandedTabMessage(this));
                }
                 */
            }
        }

        public int TabOrder
        {
            get { return Model.TabOrder; }
            set { Model.TabOrder = value; }
        }

        public string IncreaseTabOrderCommandHeader
        {
            get
            {
                if (Configuration.DockPosition.Selected == AppBarInterface.DockEdge.Top ||
                   Configuration.DockPosition.Selected == AppBarInterface.DockEdge.Bottom)
                {
                    return "Move Left";
                }

                return "Move Up";
            }
        }

        public string DecreaseTabOrderCommandHeader
        {
            get
            {
                if (Configuration.DockPosition.Selected == AppBarInterface.DockEdge.Top ||
                   Configuration.DockPosition.Selected == AppBarInterface.DockEdge.Bottom)
                {
                    return "Move Right";
                }

                return "Move Down";
            }
        }

        public ObservableCollection<ApplicationViewModel> Applications { get; private set; }
        public ListCollectionView ApplicationsView { get; private set; }
        #endregion        

        public bool Equals(TabViewModel other)
        {
            if (other == null)
                return false;
            return Model.Equals(other.Model);
        }

        public void UpdateWith(TabViewModel other)
        {
            Title = other.Title;
            IsExpanded = other.IsExpanded;
            Applications = other.Applications;
            TabOrder = other.TabOrder;
        }

        void UpdateFilter(ApplicationFilter filter)
        {
            switch (filter)
            {
                case ApplicationFilter.All:
                    ApplicationsView.Filter = null;
                    break;
                case ApplicationFilter.InstalledOnly:
                    ApplicationsView.Filter = (obj) => ((ApplicationViewModel)obj).Installed;
                    break;
                case ApplicationFilter.UninstalledOnly:
                    ApplicationsView.Filter = (obj) => !((ApplicationViewModel)obj).Installed;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        bool Validate()
        {
            if (String.IsNullOrWhiteSpace(Title))
            {
                ServiceManager.GetService<IMessageBoxService>().Show(
                    "Title not set", "Astounding Dock");
                return false;
            }

            return true;
        }

        void OnApplicationsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ApplicationViewModel application in e.NewItems)
                    {
                        Log.DebugFormat("Adding {0}", application.Title);
                        //IsExpanded = true;
                        application.Tab = Title;

                        Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Add(application.Model));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (ApplicationViewModel application in e.OldItems)
                    {
                        Log.DebugFormat("Removing {0}", application.Title);
                        Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Remove(application.Model));
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // TODO: Probably should save application positions?
                    break;
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

        public ICommand ToggleExpandCommand
        {
            get
            {
                return new RelayCommand(() => 
                    {
                        // This 'expanding' is happening outside of the normal Edit mode 
                        // so an update needs to be fired manually here.
                        IsExpanded = !IsExpanded;
                        Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Update(Model));
                    });                                       
            }
        }

        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Messenger.Default.Send<TabMessage>(TabMessage.Edit(this));
                    });
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Messenger.Default.Send<TabMessage>(TabMessage.Remove(this));
                    });
            }
        }

        public ICommand IncreaseTabOrderCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabMessage>(TabMessage.MoveUp(this));
                });
            }
        }

        public ICommand DecreaseTabOrderCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabMessage>(TabMessage.MoveDown(this));
                });
            }
        }

        public ICommand AddTabCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabToMainMessage>(TabToMainMessage.AddTab());
                });
            }
        }

        public ICommand AddApplicationCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabToMainMessage>(TabToMainMessage.AddApplication());
                });
            }
        }

        public ICommand ViewApplicationsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabToMainMessage>(TabToMainMessage.ViewApplication());
                });
            }
        }

        public ICommand OpenSettingsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabToMainMessage>(TabToMainMessage.OpenSettings());
                });
            }
        }

        public ICommand OpenAboutCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabToMainMessage>(TabToMainMessage.OpenAbout());
                });
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Messenger.Default.Send<TabToMainMessage>(TabToMainMessage.Close());
                });
            }
        }        
        #endregion
    }
}
