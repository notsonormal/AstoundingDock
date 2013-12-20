using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.ViewModels;
using GalaSoft.MvvmLight.Threading;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AstoundingDock.Messages;

namespace AstoundingApplications.AstoundingDock.Services
{
    class MessageBoxService : IMessageBoxService
    {
        MessageBoxViewModel _viewModel;

        public MessageResult Show(string message, string title)
        {
            return Show(message, title, MessageIcon.Information, MessageOptions.Okay);
        }        

        public MessageResult Show(string message, string title, MessageIcon icon)
        {
            return Show(message, title, icon, MessageOptions.Okay);
        }

        public MessageResult Show(string message, string title, MessageOptions option)
        {
            return Show(message, title, MessageIcon.Information, option);
        }

        public MessageResult Show(string message, string title, MessageIcon icon, MessageOptions option)
        {
            if (_viewModel == null)           
                _viewModel = new MessageBoxViewModel(message, title, option, icon);            
            else            
                _viewModel.Update(message, title, option, icon);            

            DispatcherHelper.UIDispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    ServiceManager.GetService<IViewService>().OpenDialog(_viewModel);
                }));

            return _viewModel.Result;
        }

        public void CloseLast()
        {
            Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(_viewModel), _viewModel);
        }
    }
}
