using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace AstoundingApplications.AstoundingDock.Messages
{
    public class RequestCloseMessage
    {
        public RequestCloseMessage(ViewModelBase viewModel, bool? dialogResult = null)
        {
            ViewModel = viewModel;
            DialogResult = dialogResult;
        }

        public ViewModelBase ViewModel { get; set; }
        public bool? DialogResult { get; set; }
    }
}
