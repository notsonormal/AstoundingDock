using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AstoundingApplications.AstoundingDock.Extensions;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Models;
using AstoundingApplications.AstoundingDock.Services;
using AstoundingApplications.AstoundingDock.Utils;
using AstoundingApplications.AstoundingDock.ViewModels;
using AstoundingApplications.Win32Interface;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace AstoundingApplications.AstoundingDock.Ui
{   
    public static class DragDropBehaviour
    {
        #region CanDrag attached dependancy property
        /// <summary>
        /// The CanDrag attached property'str name.
        /// </summary>
        public const string CanDragPropertyName = "CanDrag";

        /// <summary>
        /// Gets the value of the CanDrag attached property 
        /// for a given dependency object.
        /// </summary>
        /// <param name="obj">The object for which the property value
        /// is read.</param>
        /// <returns>The value of the CanDrag property of the specified object.</returns>
        public static bool GetCanDrag(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDragProperty);
        }

        /// <summary>
        /// Sets the value of the CanDrag attached property
        /// for a given dependency object. 
        /// </summary>
        /// <param name="obj">The object to which the property value
        /// is written.</param>
        /// <param name="value">Sets the CanDrag value of the specified object.</param>
        public static void SetCanDrag(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDragProperty, value);
        }

        /// <summary>
        /// Identifies the CanDrag attached property.
        /// </summary>
        public static readonly DependencyProperty CanDragProperty = DependencyProperty.RegisterAttached(
            CanDragPropertyName,
            typeof(bool),
            typeof(DragDropBehaviour),
            new UIPropertyMetadata(false, OnCanDragChanged));
        #endregion

        #region CanDrop attached dependancy property
        /// <summary>
        /// The CanDrop attached property'str name.
        /// </summary>
        public const string CanDropPropertyName = "CanDrop";

        /// <summary>
        /// Gets the value of the CanDrop attached property 
        /// for a given dependency object.
        /// </summary>
        /// <param name="obj">The object for which the property value
        /// is read.</param>
        /// <returns>The value of the CanDrop property of the specified object.</returns>
        public static bool GetCanDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanDropProperty);
        }

        /// <summary>
        /// Sets the value of the CanDrop attached property
        /// for a given dependency object. 
        /// </summary>
        /// <param name="obj">The object to which the property value
        /// is written.</param>
        /// <param name="value">Sets the CanDrop value of the specified object.</param>
        public static void SetCanDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(CanDropProperty, value);
        }

        /// <summary>
        /// Identifies the CanDrop attached property.
        /// </summary>
        public static readonly DependencyProperty CanDropProperty = DependencyProperty.RegisterAttached(
            CanDropPropertyName,
            typeof(bool),
            typeof(DragDropBehaviour),
            new UIPropertyMetadata(false, OnCanDropChanged));
        #endregion

        #region DragDropAction attached dependancy property
        /// <summary>
        /// The DragDropAction attached property'str name.
        /// </summary>
        public const string DragDropActionPropertyName = "DragDropAction";

        /// <summary>
        /// Gets the value of the DragDropAction attached property 
        /// for a given dependency object.
        /// </summary>
        /// <param name="obj">The object for which the property value
        /// is read.</param>
        /// <returns>The value of the DragDropAction property of the specified object.</returns>
        public static DragDropAction GetDragDropAction(DependencyObject obj)
        {
            return (DragDropAction)obj.GetValue(DragDropActionProperty);
        }

        /// <summary>
        /// Sets the value of the DragDropAction attached property
        /// for a given dependency object. 
        /// </summary>
        /// <param name="obj">The object to which the property value
        /// is written.</param>
        /// <param name="value">Sets the DragDropAction value of the specified object.</param>
        public static void SetDragDropAction(DependencyObject obj, DragDropAction value)
        {
            obj.SetValue(DragDropActionProperty, value);
        }

        /// <summary>
        /// Identifies the DragDropAction attached property.
        /// </summary>
        public static readonly DependencyProperty DragDropActionProperty = DependencyProperty.RegisterAttached(
            DragDropActionPropertyName, 
            typeof(DragDropAction),
            typeof(DragDropBehaviour), 
            new UIPropertyMetadata(null));
        #endregion

        static void OnCanDragChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = depObj as FrameworkElement;

            if (element != null)
            {
                if ((bool)e.NewValue)
                {
                    GetOrCreateAction(depObj).DragBehaviour(element, true);
                }
                else
                {
                    GetOrCreateAction(depObj).DragBehaviour(element, false);                    
                }
            }
        }

        static void OnCanDropChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = depObj as FrameworkElement;

            if (element != null)
            {
                if ((bool)e.NewValue)
                {
                    GetOrCreateAction(depObj).DropBehaviour(element, true);
                }
                else
                {
                    GetOrCreateAction(depObj).DropBehaviour(element, false);
                }
            }
        }

        static DragDropAction GetOrCreateAction(DependencyObject depObj)
        {
            DragDropAction action = depObj.GetValue(DragDropActionProperty) as DragDropAction;
            if (action == null)
            {
                action = new DragDropAction();
                depObj.SetValue(DragDropActionProperty, action);
            }
            return action;
        }
    }

    public class DragDropAction
    {
        Point _start;
        FrameworkElement _dragged;

        public void DragBehaviour(FrameworkElement element, bool enable)
        {
            if (enable)
            {
                element.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                element.PreviewMouseMove += OnPreviewMouseMove;
            }
            else
            {
                element.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                element.PreviewMouseMove -= OnPreviewMouseMove;
            }
        }

        public void DropBehaviour(FrameworkElement element, bool enable)
        {
            if (enable)
            {
                element.Drop += OnDrop;
                element.AllowDrop = true;
            }
            else
            {
                element.Drop -= OnDrop;
                element.AllowDrop = false;
            }
        }

        void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                int[] position = Win32Mouse.GetMousePosition();
                _start = new Point(position[0], position[1]);
                _dragged = element;
            }
        }

        void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null && _dragged != null)
            {
                int[] position = Win32Mouse.GetMousePosition();
                Point currentPosition = new Point(position[0], position[1]);
                Vector diff = _start - currentPosition;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) > (SystemParameters.MinimumHorizontalDragDistance) &&
                    Math.Abs(diff.Y) > (SystemParameters.MinimumVerticalDragDistance))
                {
                    DragDropEffects effects = DragDrop.DoDragDrop(element, _dragged.DataContext, DragDropEffects.Move);
                    _dragged = null;
                    e.Handled = true;
                }
            }
        }

        void OnDrop(object sender, DragEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                TabViewModel tab = element.DataContext as TabViewModel;
                if (tab == null)
                {
                    // TabViewModel is not found on the element, it's possible that the drop was done on the 'Canvas' element.
                    var tabControls = element.FindVisualChildren<TabControl>();
                    var menus = element.FindVisualChildren<Menu>();
                    var itemControls = element.FindVisualChildren<ItemsControl>();

                    if (tabControls.Count > 0 && tabControls[0].Visibility == Visibility.Visible)
                    {
                        // If currently in 'horizontal mode' add to the active tab. If there is no active tab
                        // just add to the bottom tab.
                        tab = tabControls[0].SelectedItem as TabViewModel;
                        if (tab == null)                        
                            tab = tabControls[0].Items.GetItemAt(tabControls[0].Items.Count - 1) as TabViewModel;                        
                    }
                    else if (menus.Count > 0 && menus[0].Visibility == Visibility.Visible)
                    {
                        // If currently in 'vertical mode' add to the default tab, there is no 'active' menu item after all.
                        var tabs = menus[0].Items.SourceCollection as ObservableCollection<TabViewModel>;
                        tab = tabs.SingleOrDefault(obj => obj.Title == Configuration.DefaultTab) ?? tabs.LastOrDefault();
                    }
                    else if (itemControls.Count > 0 && itemControls[0].Visibility == Visibility.Visible)
                    {
                        var window = element.FindVisualParent<Window>();
                        if (window != null && window.DataContext is MainViewModel)
                        {
                            // Add the currently expanded tab.
                            MainViewModel mainViewModel = (MainViewModel)window.DataContext;
                            tab = mainViewModel.ExpandedTab;

                            // If no tab is expanded, add to the default tab or the bottom tab.
                            if (tab == null)
                            {
                                tab = mainViewModel.Tabs.SingleOrDefault(obj => obj.Title == Configuration.DefaultTab)
                                        ?? mainViewModel.Tabs.LastOrDefault();
                            }
                        }
                    }
                }

                if (tab != null)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {                        
                        DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                            {
                                string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                                foreach (string fileName in droppedFilePaths)
                                {
                                    try
                                    {

                                        ApplicationModel model = ApplicationModel.FromFile(fileName, tab.Title);
                                        ApplicationViewModel application = new ApplicationViewModel(model);
                                        Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.Add(application, tab));
                                    }
                                    catch (FileNotFoundException ex)
                                    {
                                        ServiceManager.GetService<IMessageBoxService>().Show(
                                            "Could not add application - " + ex.Message, "Astounding Dock", MessageIcon.Error);
                                    }
                                }
                            }));
                        e.Handled = true;
                    }
                    else if (e.Data.GetDataPresent<ApplicationViewModel>())
                    {
                        DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                            {
                                ApplicationViewModel application = e.Data.GetData<ApplicationViewModel>();
                                Messenger.Default.Send<ApplicationMessage>(ApplicationMessage.Move(application, tab));
                            }));
                        e.Handled = true;
                    }
                    else
                    {
                        Debug.WriteLine("DragDropBehaviour: Unknown data droppped - " + String.Join(",", e.Data.GetFormats()));
                    }
                }
            }
        }
    }
}
