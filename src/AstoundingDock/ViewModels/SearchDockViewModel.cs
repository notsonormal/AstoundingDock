using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Utils;
using System.ComponentModel;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class SearchDockViewModel : SearchViewModel
    {
        ApplicationFilter _filter;

        public SearchDockViewModel(ObservableCollection<TabViewModel> tabs) : base(tabs)
        {
            _filter = Configuration.ApplicationFilter;

            Messenger.Default.Register<ApplicationMessage>(this, (message) =>
                {
                    switch (message.Action)
                    {
                        case ApplicationMessage.ActionType.Remove:
                            if (Applications.Contains(message.Application))
                            {
                                Applications.Remove(message.Application);
                            }
                            break;
                    }
                });
        }

        public ApplicationFilter ApplicationFilter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    Applications.Filter = FilterPredicate;
                }
            }
        }

        protected override void LoadApplications(object sender, DoWorkEventArgs e)
        {
            foreach (var tab in Tabs)
            {
                if (_loadWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                foreach (var application in tab.Applications)
                {
                    if (_loadWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    AddApplication(application);
                }
            }
        }

        protected override bool FilterPredicate(object value)
        {
            // Do 'QuickSearch' filter first.
            if (!base.FilterPredicate(value))
                return false;

            ApplicationViewModel application = (ApplicationViewModel)value;
            switch (ApplicationFilter)
            {
                case ApplicationFilter.All:
                    return true;
                case ApplicationFilter.InstalledOnly:
                    return application.Installed;
                case ApplicationFilter.UninstalledOnly:
                    return !application.Installed;
            }
            return true;
        }
    }
}
