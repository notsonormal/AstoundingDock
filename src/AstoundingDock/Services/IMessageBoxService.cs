using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.AstoundingDock.Services
{
    enum MessageResult { None, Okay, Yes, No, Cancel, Continue, Close }
    enum MessageOptions { None, Okay, Cancel, OkayCancel, YesNo, YesNoCancel, ContinueClose }
    enum MessageIcon { None, Question, Information, Warning, Error }

    interface IMessageBoxService : IService
    {
        MessageResult Show(string message, string title);        
        MessageResult Show(string message, string title, MessageIcon icon);
        MessageResult Show(string message, string title, MessageOptions option);
        MessageResult Show(string message, string title, MessageIcon icon, MessageOptions option);
        void CloseLast();
    }
}
