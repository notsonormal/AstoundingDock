using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using AstoundingApplications.AstoundingDock.Extensions;
using AstoundingApplications.AstoundingDock.Services;
using System.Windows;

namespace AstoundingApplications.AstoundingDock.Converters
{
    /// <summary>
    /// Determines where or not a button should be visible, depending on what option was selected for the message
    /// box and what button it is.
    /// </summary>
    class MessageBoxButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string buttonType = parameter as string;
            if (value != null && buttonType != null)
            {
                MessageOptions option = EnumEx.Parse<MessageOptions>(value);
                switch (option)
                {
                    case MessageOptions.Okay:
                        if (buttonType == "Okay")
                            return Visibility.Visible;
                        return Visibility.Collapsed;
                    case MessageOptions.Cancel:
                        if (buttonType == "Cancel")
                            return Visibility.Visible;
                        return Visibility.Collapsed;
                    case MessageOptions.OkayCancel:
                        if (buttonType == "Okay" || buttonType == "Cancel")
                            return Visibility.Visible;
                        return Visibility.Collapsed;
                    case MessageOptions.YesNo:
                        if (buttonType == "Yes" || buttonType == "No")
                            return Visibility.Visible;
                        return Visibility.Collapsed;
                    case MessageOptions.YesNoCancel:
                        if (buttonType == "Yes" || buttonType == "No" || buttonType == "Cancel")
                            return Visibility.Visible;
                        return Visibility.Collapsed;
                    case MessageOptions.ContinueClose:
                        if (buttonType == "Continue" || buttonType == "Close")
                            return Visibility.Visible;
                        return Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
