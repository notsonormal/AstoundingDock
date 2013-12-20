using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using AstoundingApplications.AstoundingDock.Extensions;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Services;
using AstoundingApplications.AstoundingDock.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class SearchViewModel : ViewModelBase
    {
        string _quickSearchText;   
        ObservableCollection<ApplicationViewModel> _applications;
        protected readonly BackgroundWorker _loadWorker;

        public SearchViewModel(ObservableCollection<TabViewModel> tabs)
        {
            Tabs = tabs;
            SelectedTab = Tabs.SingleOrDefault(tabElem => tabElem.Title == Configuration.DefaultTab) ?? Tabs.FirstOrDefault();
            _applications = new ObservableCollection<ApplicationViewModel>();
            Applications = (ListCollectionView) CollectionViewSource.GetDefaultView(_applications);
            Applications.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
            Applications.Filter = FilterPredicate;

            SelectedApplications = new ObservableCollection<ApplicationViewModel>();

            _loadWorker = new BackgroundWorker();
            _loadWorker.WorkerSupportsCancellation = true;
            _loadWorker.WorkerReportsProgress = true;
            _loadWorker.DoWork += LoadApplications;
            _loadWorker.ProgressChanged += LoadingProgressChanged;
            _loadWorker.RunWorkerCompleted += LoadingComplete;

            _loadWorker.RunWorkerAsync();
        }

        #region Properties
        public TabViewModel SelectedTab { get; set; }
        public ObservableCollection<ApplicationViewModel> SelectedApplications { get; private set; }
        public ListCollectionView Applications { get; private set; }
        public ObservableCollection<TabViewModel> Tabs { get; private set; }

        public string QuickSearchText
        {
            get { return _quickSearchText; }
            set
            {
                if (value != _quickSearchText)
                {
                    _quickSearchText = value;
                    Applications.Filter = FilterPredicate;
                }
            }
        }
        #endregion        

        protected virtual bool FilterPredicate(object value)
        {
            ApplicationViewModel application = value as ApplicationViewModel;

            // Returns true if...
            // 1) QuickSearch text is empty or
            // 2) The title of the application contains that text or
            // 3) The file path of the application contains that text
            return application != null && (
                        String.IsNullOrWhiteSpace(QuickSearchText) ||
                        application.Title.Contains(QuickSearchText, StringComparison.InvariantCultureIgnoreCase) ||
                        application.FilePath.Contains(QuickSearchText, StringComparison.InvariantCultureIgnoreCase));
        }

        protected virtual void LoadApplications(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void AddApplication(ApplicationViewModel addThis)
        {
            _loadWorker.ReportProgress(0, addThis);
        }

        void LoadingProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var addThis = (ApplicationViewModel)e.UserState;

            if (_applications != null)
            {
                if (!_applications.Contains(addThis))
                    _applications.Add(addThis);
            }
        }

        void LoadingComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            _loadWorker.DoWork -= LoadApplications;
            _loadWorker.ProgressChanged -= LoadingProgressChanged;
            _loadWorker.RunWorkerCompleted -= LoadingComplete;
            _loadWorker.Dispose();

            if (e.Error != null)            
                throw e.Error;            
        }

        public override void Cleanup()
        {
            if (_loadWorker.IsBusy)
                _loadWorker.CancelAsync();

            if (_applications != null)
            {
                Applications.Filter = null;
                SelectedApplications.Clear();
                _applications.Clear();
                _applications = null;
                Applications.Refresh();
            }

            base.Cleanup();
        }

        #region Commands
        public ICommand AddSelectedCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        if (SelectedApplications.Count > 0)
                        {
                            string title = "Astounding Dock - Add " + SelectedApplications.Count + " applications?";
                            StringBuilder message = new StringBuilder();
                            message.Append("Add these applications to the ").Append(SelectedTab.Title).Append(" tab?\n\n");
                            foreach (var application in SelectedApplications)
                            {
                                message.Append(application.Title).Append("\n");
                            }

                            var result = ServiceManager.GetService<IMessageBoxService>().Show(
                                message.ToString(), title, MessageIcon.Question, MessageOptions.YesNo);

                            if (result == MessageResult.Yes)
                            {
                                foreach (var selected in SelectedApplications.ToList())
                                {
                                    SelectedApplications.Remove(selected);
                                    Applications.Remove(selected);
                                    Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.Add(selected, SelectedTab, true));
                                }
                            }
                        }
                    });
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Cleanup();                 
                    Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                });
            }
        }

        public ICommand CleanupCommand
        {
            get
            {
                return new RelayCommand(Cleanup);
            }
        }

        #endregion
    }
}
