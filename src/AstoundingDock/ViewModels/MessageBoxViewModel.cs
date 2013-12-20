using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AstoundingApplications.AstoundingDock.Services;
using System.Drawing;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using AstoundingApplications.AstoundingDock.Messages;
using System.Windows;

namespace AstoundingApplications.AstoundingDock.ViewModels
{
    class MessageBoxViewModel : ViewModelBase
    {
        public MessageBoxViewModel(string message, string title, MessageOptions option, MessageIcon icon)
        {
            Update(message, title, option, icon);
        }

        public void Update(string message, string title, MessageOptions option, MessageIcon icon)
        {
            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentException("message");
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentException("title");
            if (option == MessageOptions.None)
                throw new ArgumentException("option");
            if (icon == MessageIcon.None)
                throw new ArgumentException("icon");

            Title = title;
            Message = message;
            Option = option;
            Result = MessageResult.None;

            switch (icon)
            {
                case MessageIcon.Question:
                    Icon = SystemIcons.Question;
                    break;
                case MessageIcon.Information:
                    Icon = SystemIcons.Information;
                    break;
                case MessageIcon.Warning:
                    Icon = SystemIcons.Warning;
                    break;
                case MessageIcon.Error:
                    Icon = SystemIcons.Error;
                    break;
                default:
                    throw new NotImplementedException(icon.ToString());
            }
        }

        #region Properties
        public string Message { get; set; }
        public string Title { get; set; }
        public MessageOptions Option { get; set; }
        public Icon Icon { get; set; }
        public MessageResult Result { get; private set; }
        #endregion

        public override void Cleanup()
        {
            Icon.Dispose();
            base.Cleanup();
        }

        #region Commands
        public ICommand OkayCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Cleanup();
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
                        Cleanup();
                        Result = MessageResult.Cancel;
                        Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                    });
            }
        }

        public ICommand YesCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Cleanup();
                        Result = MessageResult.Yes;
                        Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                    });
            }
        }

        public ICommand NoCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Cleanup();
                        Result = MessageResult.No;
                        Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                    });
            }
        }

        public ICommand ContinueCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Cleanup();
                    Result = MessageResult.Continue;
                    Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
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
                    Result = MessageResult.Close;
                    Messenger.Default.Send<RequestCloseMessage>(new RequestCloseMessage(this), this);
                });
            }
        }

        public ICommand CopyMessageCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Clipboard.SetData(DataFormats.Text, (object)Message);
                    });
            }
        }
         
        #endregion
    }
}
