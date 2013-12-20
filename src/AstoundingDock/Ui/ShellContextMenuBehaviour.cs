using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AstoundingApplications.AstoundingDock.ViewModels;
using System.IO.Tools;
using System.IO;
using AstoundingApplications.Win32Interface;
using AstoundingApplications.AstoundingDock.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Interop;
using System.Diagnostics;

namespace AstoundingApplications.AstoundingDock.Ui
{
    class ShellContextMenuBehaviour
    {
        public static bool GetOpenShellContextMenu(Control control)
        {
            return (bool)control.GetValue(OpenShellContextMenuProperty);
        }

        public static void SetOpenShellContextMenu(Control control, bool value)
        {
            control.SetValue(OpenShellContextMenuProperty, value);
        }

        public static readonly DependencyProperty OpenShellContextMenuProperty = DependencyProperty.RegisterAttached(
            "OpenShellContextMenu", typeof(bool), typeof(ShellContextMenuBehaviour), new UIPropertyMetadata(false, OnOpenShellContextMenuPropertyChanged));

        static void OnOpenShellContextMenuPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Control item = depObj as Control;

            if (item != null)
            {
                if ((bool)e.NewValue)
                {
                    item.ContextMenuOpening += OnContextMenuOpening;
                }
                else
                {
                    item.ContextMenuOpening -= OnContextMenuOpening; 
                }
            }
        }

        /// <summary>
        /// Opens the shell context menu instead of the normal context menu on right click.
        /// </summary>
        static void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Control control = sender as Control;

            // Attempt to open the explorer context menu for the application. If the file does not exist or 
            // there is some other problem then a context menu (defined in the XAML) with just "Edit Entry" 
            // and "Remove Entry" is opened instead.
            if (control != null && control.DataContext is ApplicationViewModel)
            {
                ApplicationViewModel application = (ApplicationViewModel)control.DataContext;

                if (File.Exists(application.FilePath))
                {
                    ContextMenuWrapper cmw = new ContextMenuWrapper();
                    cmw.OnQueryMenuItems += (QueryMenuItemsEventHandler)delegate(object s, QueryMenuItemsEventArgs args)
                    {
                        args.ExtraMenuItems = new string[] { "Edit Dock Entry", "Remove Dock Entry", "---" };

                        args.GrayedItems = new string[] { "delete", "rename", "cut", "copy" };
                        args.HiddenItems = new string[] { "link" };
                        args.DefaultItem = 1;
                    };
                    cmw.OnAfterPopup += (AfterPopupEventHandler) delegate(object s, AfterPopupEventArgs args)
                    {
                        Messenger.Default.Send<ShellContextMenuMessage>(ShellContextMenuMessage.Closed());
                    };

                    Messenger.Default.Send<ShellContextMenuMessage>(ShellContextMenuMessage.Opened());

                    try
                    {
                        FileSystemInfoEx[] files = new[] { FileInfoEx.FromString(application.FilePath) };
                        int[] position = Win32Mouse.GetMousePosition();
                        string command = cmw.Popup(files, new System.Drawing.Point(position[0], position[1]));

                        // Handle the click on the 'ExtraMenuItems'.
                        switch (command)
                        {
                            case "Edit Dock Entry":
                                Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.Edit(application));
                                break;
                            case "Remove Dock Entry":
                                Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.Remove(application));
                                break;
                        }
                        e.Handled = true; // Don't open the normal context menu.
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Problem displaying shell context menu: {0}", ex);
                    }
                }
            }
        }

    }
}
