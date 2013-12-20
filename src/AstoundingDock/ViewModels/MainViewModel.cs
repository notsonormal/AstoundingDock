using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;
using System.Windows.Input;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Models;
using AstoundingApplications.AstoundingDock.Services;
using AstoundingApplications.AstoundingDock.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Reactive.Concurrency;
using System.Collections.Generic;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        readonly XmlDatabase _database;
        TabViewModel _expandedTab;

        public MainViewModel()
        {
        }

        public MainViewModel(XmlDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            _database = database;
            Tabs = new ObservableCollection<TabViewModel>();
            TabsView = (ListCollectionView) CollectionViewSource.GetDefaultView(Tabs);
            TabsView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));

            // Loading applications asynchronously using reactive extensions to reduce initial startup time.
            
            // TODO: Does this actually speed things up or is it loading all the sections before add doing
            // the subscribe??
            _database.LoadTabs().ToObservable().Subscribe(tabModel =>
            {
                TabViewModel tabViewModel = new TabViewModel(tabModel);
                Tabs.Add(tabViewModel);
                Configuration.AvailableTabs.Add(tabViewModel.Title);
            },
            () => // OnComplete
            {
                Tabs.CollectionChanged += OnTabsChanged;

                // Expand the default tab when the application starts up.
                ExpandedTab = Tabs.SingleOrDefault(obj => obj.Title == Configuration.DefaultTab) ?? Tabs.FirstOrDefault();
            });
             
            Messenger.Default.Register<ApplicationMessage>(this, OnApplicationMessage);
            Messenger.Default.Register<TabMessage>(this, OnTabMessage);
            Messenger.Default.Register<TabToMainMessage>(this, OnTabToMainMessage);
        }

        public ObservableCollection<TabViewModel> Tabs { get; private set; }
        public ListCollectionView TabsView { get; private set; }
        public TabViewModel ExpandedTab
        {
            get { return _expandedTab; }
            set
            {
                if (_expandedTab != value)
                {
                    if (_expandedTab != null)
                        _expandedTab.IsExpanded = false;

                    if (value != null)
                        value.IsExpanded = true;

                    _expandedTab = value;
                }
            }
        }

        void OnTabsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (TabViewModel tab in e.NewItems)
                    {
                        Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Add(tab.Model));
                        Configuration.AvailableTabs.Add(tab.Title);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (TabViewModel tab in e.OldItems)
                    {
                        Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Remove(tab.Model));
                        Configuration.AvailableTabs.Remove(tab.Title);
                        Messenger.Default.Unregister(tab);

                        // Removing items one-by-one so that the NotifyCollectionChangedAction.Remove
                        // event is fired for each of them in the Applications collection.
                        foreach (var application in tab.Applications.ToList())
                        {
                            tab.Applications.Remove(application);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    // TODO:
                    break;
            }
        }

        void OnApplicationMessage(ApplicationMessage message)
        {
            switch (message.Action)
            {
                case ApplicationMessage.ActionType.Add:
                    Add(message.Application, message.Tab, message.SuppressMessage);
                    break;
                case ApplicationMessage.ActionType.AddNew:
                    AddNew();
                    break;
                case ApplicationMessage.ActionType.Edit:
                    Edit(message.Application);
                    break;
                case ApplicationMessage.ActionType.Move:
                    Move(message.Application, message.Tab);
                    break;
                case ApplicationMessage.ActionType.Remove:
                    Remove(message.Application);
                    break;
            }
        }

        void OnTabMessage(TabMessage message)
        {
            switch (message.Action)
            {
                case TabMessage.ActionType.Add:
                    Add(message.Tab);
                    break;
                case TabMessage.ActionType.Edit:
                    Edit(message.Tab);
                    break;
                case TabMessage.ActionType.Remove:
                    Remove(message.Tab);
                    break;
                case TabMessage.ActionType.Move:
                    Move(message.Tab, message.TabB);
                    break;
            }
        }

        void OnTabToMainMessage(TabToMainMessage message)
        {
            switch (message.Action)
            {
                case TabToMainMessage.ActionType.AddTab:
                    Add(new TabViewModel());
                    break;
                case TabToMainMessage.ActionType.AddApplication:
                    ServiceManager.GetService<IViewService>().OpenDialog(new SearchNewViewModel(Tabs));
                    break;
                case TabToMainMessage.ActionType.ViewApplications:
                    ServiceManager.GetService<IViewService>().OpenDialog(new SearchDockViewModel(Tabs));
                    break;
                case TabToMainMessage.ActionType.OpenAbout:
                    ServiceManager.GetService<IViewService>().OpenDialog(new AboutViewModel());
                    break;
                case TabToMainMessage.ActionType.OpenSettings:
                    ServiceManager.GetService<IViewService>().OpenDialog(new SettingsViewModel());
                    break;
                case TabToMainMessage.ActionType.Close:
                    Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                    break;
            }
        }

        void AddNew()
        {
            ApplicationViewModel application = new ApplicationViewModel()
            {
                Tab = Configuration.DefaultTab,
                Installed = true
            };

            AddNew(application);
        }

        void AddNew(ApplicationViewModel addThis)
        {
            if (addThis == null)
                throw new ArgumentNullException("addThis");

            ServiceManager.GetService<IViewService>().OpenDialog(addThis);
            if (addThis.Result != MessageResult.Okay)
                return; // Cancelled add          

            // Check if any other applications have the same title.
            if (Tabs.Any(tabElem => tabElem.Applications.Any(
                applicationElem => applicationElem.Title == addThis.Title)))
            {
                ServiceManager.GetService<IMessageBoxService>().Show(
                    "Application already exists with that title", 
                    "Astounding Dock", MessageIcon.Error);

                // Re-open dialog.
                AddNew(addThis);
                return;
            }

            var addToThis = Tabs.Single(tabElem => tabElem.Title == addThis.Tab);
            Add(addThis, addToThis);
        }

        void Add(ApplicationViewModel addThis, TabViewModel addToThis)
        {
            Add(addThis, addToThis, false);
        }

        void Add(ApplicationViewModel addThis, TabViewModel addToThis, bool suppressDialog)
        {
            if (addThis == null)
                throw new ArgumentNullException("addThis");
            if (addToThis == null)
                throw new ArgumentNullException("addToThis");

            var tab = Tabs.SingleOrDefault(tabElem => tabElem.Applications.Contains(addThis));
            if (tab != null)
            {
                ServiceManager.GetService<IMessageBoxService>().Show(String.Format(
                    "{0} already added to tab {1}", addThis.Title, tab.Title),
                    "Astounding Dock", MessageIcon.Error);
                return;
            }

            addToThis.Applications.Add(addThis);

            if (!suppressDialog)
            {
                ServiceManager.GetService<IMessageBoxService>().Show(String.Format(
                    "Added application {0} to {1}", addThis.Title, addToThis.Title), "Astounding Dock");
            }
        }

        void Edit(ApplicationViewModel toEdit)
        {
            if (toEdit == null)
                throw new ArgumentNullException("toEdit");

            ApplicationViewModel editApplication = new ApplicationViewModel();
            editApplication.UpdateWith(toEdit);

            ServiceManager.GetService<IViewService>().OpenDialog(editApplication);

            if (editApplication.Result != MessageResult.Okay)
                return; // Cancelled edit.

            if (!editApplication.Title.Equals(toEdit.Title, StringComparison.InvariantCultureIgnoreCase))
            {
                // Application title has changed, check if any other applications have the new title
                if (Tabs.Any(t => t.Applications.Any(a => a.Title == editApplication.Title)))
                {
                    ServiceManager.GetService<IMessageBoxService>().Show(
                        "Already exists with the title " + editApplication.Title,
                        "Astounding Dock", MessageIcon.Error);

                    // Re-open dialog.                    
                    Edit(toEdit);
                    return;
                }
            }

            toEdit.UpdateWith(editApplication);
            if (toEdit.Tab != toEdit.OldTab)
            {
                // Moving to a new tab.
                var newTab = Tabs.Single(t => t.Title == toEdit.Tab);
                Move(toEdit, newTab);
            }
            else
            {
                Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Update(toEdit.Model));
            }

            // Refresh the view to reapply the sort in case the application was renamed.
            // Needs to be done after 'UpdateWith' is called.
            Tabs.Single(t => t.Title == editApplication.Tab).ApplicationsView.Refresh();
        }

        void Move(ApplicationViewModel toMove, TabViewModel movingTo)
        {
            if (toMove == null)
                throw new ArgumentNullException("toMove");
            if (movingTo == null)
                throw new ArgumentNullException("movingTo");

            if (!movingTo.Applications.Contains(toMove))
            {
                var movingFrom = Tabs.Single(sectionElem => sectionElem.Applications.Contains(toMove));

                movingFrom.Applications.Remove(toMove);
                movingTo.Applications.Add(toMove);
            }
        }

        void Remove(ApplicationViewModel removeThis)
        {
            if (removeThis == null)
                throw new ArgumentNullException("removeThis");            

            var result = ServiceManager.GetService<IMessageBoxService>().Show(String.Format(
                "Are you sure you wish to remove {0}?", removeThis.Title), "Astounding Dock",
                MessageIcon.Question, MessageOptions.YesNo);

            if (result == MessageResult.Yes)
            {
                var section = Tabs.Single(sectionElem => sectionElem.Applications.Contains(removeThis));
                section.Applications.Remove(removeThis);
            }
        }

        void Add(TabViewModel addThis)
        {
            if (addThis == null)
                throw new ArgumentNullException("addThis");  

            ServiceManager.GetService<IViewService>().OpenDialog(addThis);
            if (addThis.Result != MessageResult.Okay)
                return; // Cancelled add

            // Check if any other tabs have this title.
            if (Tabs.Any(sectionElem => sectionElem.Title == addThis.Title))
            {
                ServiceManager.GetService<IMessageBoxService>().Show(
                    "Tab with that title already exists", "Astounding Dock", MessageIcon.Error);

                // Re-open dialog. 
                Add(addThis);
                return;
            }

            Tabs.Add(addThis);
        }

        void Edit(TabViewModel toEdit)
        {
            if (toEdit == null)
                throw new ArgumentNullException("toEdit");  

            TabViewModel editTab = new TabViewModel();
            editTab.UpdateWith(toEdit);

            ServiceManager.GetService<IViewService>().OpenDialog(editTab);

            if (editTab.Result != MessageResult.Okay)
                return; // Cancelled edit

            if (!editTab.Title.Equals(toEdit.Title, StringComparison.InvariantCultureIgnoreCase))
            {
                // Tab title changed, check if any other tabs have this title.
                if (Tabs.Any(t => t.Title == editTab.Title))
                {                
                    ServiceManager.GetService<IMessageBoxService>().Show(
                        "Tab wit that title already exists", "Astounding Dock", MessageIcon.Error);

                    // Re-open dialog. 
                    Edit(toEdit);
                    return;
                }                
            }

            toEdit.UpdateWith(editTab);
            Messenger.Default.Send<DatabaseMessage>(DatabaseMessage.Update(toEdit.Model));

            // Refresh the view to reapply the sort in case the tab was renamed.
            // Needs to be done after 'UpdateWith' is called.
            TabsView.Refresh();
        }

        void Remove(TabViewModel removeThis)
        {
            if (removeThis == null)
                throw new ArgumentNullException("removeThis");  

            var result = ServiceManager.GetService<IMessageBoxService>().Show(String.Format(
                "Are you sure you wish to remove {0}?", removeThis.Title), "Astounding Dock",
                MessageIcon.Question, MessageOptions.YesNo);

            if (result != MessageResult.Yes)
                return;
            
            if (removeThis.Title == Configuration.DefaultTab)
            {
                ServiceManager.GetService<IMessageBoxService>().Show(
                    "Cannot remove the default section", "Astounding Dock", MessageIcon.Error);
                return;
            }

            if (removeThis.Applications.Count > 0)
            {
                var result2 = ServiceManager.GetService<IMessageBoxService>().Show(String.Format(
                    "This tab has {0} applications, remove anyway?", removeThis.Applications.Count),
                    "Astounding Dock", MessageIcon.Question, MessageOptions.YesNo);

                if (result2 != MessageResult.Yes)
                    return;
            }

            Tabs.Remove(removeThis);            
        }

        void Move(TabViewModel moveThis, TabViewModel moveToThis)
        {
            if (moveThis == null)
                throw new ArgumentNullException("moveThis");
            if (moveToThis == null)
                throw new ArgumentNullException("moveToThis");  

            Tabs.Move(Tabs.IndexOf(moveThis), Tabs.IndexOf(moveToThis));
        }

        #region Commands
        public ICommand AddTabCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Add(new TabViewModel());
                    });
            }
        }

        public ICommand OpenAboutCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        ServiceManager.GetService<IViewService>().OpenWindow(new AboutViewModel());
                    });   
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

        public ICommand OpenSettingsWindowCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        ServiceManager.GetService<IViewService>().OpenDialog(new SettingsViewModel());
                    });
            }
        }

        public ICommand AddApplicationCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        ServiceManager.GetService<IViewService>().OpenDialog(new SearchNewViewModel(Tabs));
                    });
            }
        }

        public ICommand ViewApplicationsCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        ServiceManager.GetService<IViewService>().OpenDialog(new SearchDockViewModel(Tabs));
                    });
            }
        }
        #endregion
    }
}