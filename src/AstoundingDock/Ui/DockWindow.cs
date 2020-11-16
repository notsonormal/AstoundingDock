using AstoundingApplications.AppBarInterface;
using AstoundingApplications.AstoundingDock.Messages;
using AstoundingApplications.AstoundingDock.Utils;
using AstoundingApplications.Win32Interface;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ListBox = System.Windows.Controls.ListBox;
using Menu = System.Windows.Controls.Menu;

namespace AstoundingApplications.AstoundingDock.Ui
{
    public class DockWindow : Window
    {        
        #region ActiveScreen DependancyProperty
        /// <summary>
        /// The <see cref="ActiveScreen" /> dependency property'str name.
        /// </summary>
        public const string ActiveScreenPropertyName = "ActiveScreen";

        /// <summary>
        /// Gets or sets the value of the <see cref="ActiveScreen" />
        /// property. This is a dependency property.
        /// </summary>
        public Screen ActiveScreen
        {
            get
            {
                return (Screen)GetValue(ActiveScreenProperty);
            }
            set
            {
                SetValue(ActiveScreenProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ActiveScreen" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveScreenProperty = DependencyProperty.Register(
            ActiveScreenPropertyName,
            typeof(Screen),
            typeof(DockWindow),
            new UIPropertyMetadata(Screen.PrimaryScreen));
        #endregion

        #region IconRows dependancy property
        /// <summary>
        /// The <see cref="IconRows" /> dependency property'str name.
        /// </summary>
        public const string IconRowsPropertyName = "IconRows";

        /// <summary>
        /// Gets or sets the value of the <see cref="IconRows" />
        /// property. This is a dependency property.
        /// </summary>
        public int IconRows
        {
            get
            {
                return (int)GetValue(IconRowsProperty);
            }
            set
            {
                SetValue(IconRowsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IconRows" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconRowsProperty = DependencyProperty.Register(
            IconRowsPropertyName,
            typeof(int),
            typeof(DockWindow),
            new UIPropertyMetadata(3));
        #endregion

        #region DockPosition dependancy property
        /// <summary>
        /// The <see cref="DockPosition" /> dependency property'str name.
        /// </summary>
        public const string DockPositionPropertyName = "DockPosition";

        /// <summary>
        /// Gets or sets the value of the <see cref="DockPosition" />
        /// property. This is a dependency property.
        /// </summary>
        public DockPosition DockPosition
        {
            get
            {
                return (DockPosition)GetValue(DockedEdgeProperty);
            }
            set
            {
                SetValue(DockedEdgeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DockPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DockedEdgeProperty = DependencyProperty.Register(
            DockPositionPropertyName,
            typeof(DockPosition),
            typeof(DockWindow),
            new UIPropertyMetadata(new DockPosition(DockEdge.Right)));
        #endregion

        #region AutoHide dependancy property
        /// <summary>
        /// The <see cref="AutoHide" /> dependency property'str name.
        /// </summary>
        public const string AutoHidePropertyName = "AutoHide";

        /// <summary>
        /// Gets or sets the value of the <see cref="AutoHide" />
        /// property. This is a dependency property.
        /// </summary>
        public bool AutoHide
        {
            get
            {
                return (bool)GetValue(AutoHideProperty);
            }
            set
            {
                SetValue(AutoHideProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="AutoHide" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(
            AutoHidePropertyName,
            typeof(bool),
            typeof(DockWindow),
            new UIPropertyMetadata(true));
        #endregion

        #region AutoHideDelay dependancy property
        /// <summary>
        /// The <see cref="AutoHideDelay" /> dependency property'str name.
        /// </summary>
        public const string AutoHideDelayPropertyName = "AutoHideDelay";

        /// <summary>
        /// Gets or sets the value of the <see cref="AutoHideDelay" />
        /// property. This is a dependency property.
        /// </summary>
        public int AutoHideDelay
        {
            get
            {
                return (int)GetValue(AutoHideDelayProperty);
            }
            set
            {
                SetValue(AutoHideDelayProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="AutoHideDelay" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideDelayProperty = DependencyProperty.Register(
            AutoHideDelayPropertyName,
            typeof(int),
            typeof(DockWindow),
            new UIPropertyMetadata(1000));
        #endregion

        #region PopupDelay dependancy property
        /// <summary>
        /// The <see cref="PopupDelay" /> dependency property'str name.
        /// </summary>
        public const string PopupDelayPropertyName = "PopupDelay";

        /// <summary>
        /// Gets or sets the value of the <see cref="PopupDelay" />
        /// property. This is a dependency property.
        /// </summary>
        public int PopupDelay
        {
            get
            {
                return (int)GetValue(PopupDelayProperty);
            }
            set
            {
                SetValue(PopupDelayProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PopupDelay" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupDelayProperty = DependencyProperty.Register(
            PopupDelayPropertyName,
            typeof(int),
            typeof(DockWindow),
            new UIPropertyMetadata(1000));
        #endregion

        #region Fields
        static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly TimeSpan SlideDuration = TimeSpan.FromMilliseconds(300);
        static readonly object _isStealingFocusLocker = new object();

        Grid _mainGrid;
        Menu _verticalMenu;
        ListBox _horizontalListBox;
        IAppBar _appbar;
        bool _isContextMenuOpen;
        bool _isMenuOpen;
        bool _isShellContextMenuOpen;        
        ActionQueue _actionQueue;
        bool _isStealingFocus;
        #endregion

        public DockWindow()
        {
            try
            {

                this.Closing += OnClosing;
                this.Loaded += OnLoaded;

                _actionQueue = new ActionQueue();
                IconRows = Configuration.IconRows;
                ActiveScreen = Configuration.ActiveScreen;
                DockPosition = Configuration.DockPosition;
                AutoHide = Configuration.AutoHide;
                PopupDelay = Configuration.PopupDelay;
                AutoHideDelay = Configuration.AutoHideDelay;

                _appbar = new WpfAppBar(this)
                {
                    AutoHideDelay = AutoHideDelay,
                    PopupDelay = PopupDelay,
                    ActiveScreen = ActiveScreen,
                    ReserveScreen = Configuration.ReserveScreen
                };
                _appbar.AppBarEvent += OnAppBarEvent;

                Observable.FromEventPattern(this, "ContextMenuOpening").Subscribe(ep => _isContextMenuOpen = true);
                Observable.FromEventPattern(this, "ContextMenuClosing").Subscribe(ep => _isContextMenuOpen = false);
            }
            catch(Exception ex)
            {
                Log.Error(string.Format("Exception when creating the dock window, ErrorMessage: {0}, StackTrack: {1}",
                    ex.Message, ex.StackTrace), ex);
                throw;
            }
        }

        bool IsStealingFocus
        {
            get
            {
                lock (_isStealingFocusLocker)
                {
                    return _isStealingFocus;
                }
            }
            set
            {
                lock (_isStealingFocusLocker)
                {
                    _isStealingFocus = value;
                }
            }
        }

        public void PopupWindow()
        {
            Log.DebugFormat("PopupWindow");
            ShowWindow();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Log.DebugFormat("OnLoaded");
            this.Loaded -= OnLoaded;

            _mainGrid = (Grid)FindName("mainGrid");
            _horizontalListBox = (ListBox)FindName("horizontalListBox");
            _verticalMenu = (Menu)FindName("verticalMenu");

            _appbar.Init();
            _appbar.Dock(DockPosition);

            Messenger.Default.Register<SettingChangedMessage>(this, OnSettingChanged);
            Messenger.Default.Register<ShellContextMenuMessage>(this, (message) => _isShellContextMenuOpen = message.IsOpen);
            Messenger.Default.Register<MenuMessage>(this, (message) => _isMenuOpen = message.IsOpen);

            _actionQueue.QueueAction(MinimizeWindow);
            ShowWindow();
        }

        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Log.DebugFormat("OnClosing");
            _appbar.AppBarEvent -= OnAppBarEvent;
            this.Closing -= OnClosing;            
        }

        void OnAppBarEvent(object sender, AppBarEventArgs e)
        {
            Thread.Sleep(10);
            Debug.Print("OnAppBarEvent, Action: {0}, Data: {1}", e.Action, e.Data);

            switch (e.Action)
            {
                case AppBarNotificationAction.ErrorEvent:
                    Log.Warn(e.Data);
                    break;
                case AppBarNotificationAction.ShowWindow:
                    ShowWindow();
                    break;
                case AppBarNotificationAction.HideWindow:
                    HideWindow();
                    break;
                case AppBarNotificationAction.PositionChanged:
                case AppBarNotificationAction.StateChanged:
                    Log.Debug("PositionChanged/StateChanged");
                    HideWindow();
                    ShowWindow();
                    break;
                case AppBarNotificationAction.FullScreenApp:
                    OnFullScreenWindowChanged((bool)e.Data);
                    break;
            }
        }

        void OnFullScreenWindowChanged(bool isFullScreen)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (isFullScreen)
                {
                    Log.Debug("FullScreenApp opened");

                    // Windows seems to automatically push the dock into the background when a full screen 
                    // application is opened
                }
                else
                {
                    Log.Debug("FullScreenApp closed");

                    // When the full screen application is closed the dock is still underneath all the applicationsn
                    // and even the desktop. This is fine when in autohide mode but is a problem otherwise. 
                    
                    // If we set the width/height to 0 and then do the slide out action, it looks the dock was hidden
                    // of the screen when the other application was in full screen and we just putting dock back
                    if (_appbar.ReserveScreen || !AutoHide)
                    {
                        Width = 0;
                        Height = 0;
                        _actionQueue.QueueAction(SlideOut);
                    }           
                }
            });
        }

        void OnSettingChanged(SettingChangedMessage message)
        {
            switch (message.Name)
            {
                case "IconRows":
                    int iconRows = (int)message.Value;
                    bool slideIn = iconRows < IconRows;

                    ClearAnimations();
                                      
                    IconRows = iconRows;

                    /*
                    Position position = CalculateSlideOutPosition();
                    //Position position = slideIn ? CalculateSlideInPosition() : CalculateSlideOutPosition();
                    //Position position = new Position();

                    if (_appbar.ReserveScreen)
                    {
                        Width = 0;
                        Height = 0;

                        _appbar.Resize(ref position);
                    }

                    if (slideIn)
                        _actionQueue.QueueAction(SlideOut);
                    else
                        _actionQueue.QueueAction(SlideOut);
                     */

                    if (_appbar.ReserveScreen)
                    {
                        Position position = CalculateSlideOutPosition();

                        Width = 0;
                        Height = 0;

                        _appbar.Resize(ref position);
                    }

                    _actionQueue.QueueAction(SlideOut);
                    break;
                case "ActiveScreen":
                    ActiveScreen = Helper.GetScreenFromName((string)message.Value);
                    _appbar.ActiveScreen = ActiveScreen;

                    _actionQueue.QueueAction(SlideInMinimize);
                    _actionQueue.QueueAction(MinimizeWindow);
                    _actionQueue.QueueAction(SlideOut);                    
                    break;
                case "DockPosition":
                    DockPosition oldValue = DockPosition;
                    DockPosition newValue = (DockPosition)message.Value;

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Log.DebugFormat("ChangeDockPosition, {0} to {1}", oldValue, newValue);

                        DockPosition = newValue;
                        _appbar.Dock(DockPosition);

                        _actionQueue.QueueAction(SlideInMinimize, oldValue);
                        _actionQueue.QueueAction(MinimizeWindow);
                        _actionQueue.QueueAction(SlideOut);                        
                    });                    
                    break;
                case "AutoHide":
                    AutoHide = (bool)message.Value;
                    break;
                case "AutoHideDelay":
                    // TODO: I'm not even saving it in the dependancy property, maybe I 
                    // should remove the  dependancy property?
                    _appbar.AutoHideDelay = (int)message.Value;
                    break;
                case "PopupDelay":
                    // TODO: I'm not even saving it in the dependancy property, maybe I 
                    // should remove the  dependancy property?
                    _appbar.PopupDelay = (int)message.Value;
                    break;  
                case "ReserveScreen":
                    _appbar.ReserveScreen = (bool)message.Value;                    
                    if (_appbar.ReserveScreen)
                    {
                        _actionQueue.QueueAction(MinimizeWindow);
                        _actionQueue.QueueAction((parameter) =>
                            {
                                _appbar.Reserve();
                                //HandleFullScreenRdpApps();
                            });
                        _actionQueue.QueueAction(SlideOut);
                    }
                    else
                    {
                        _actionQueue.QueueAction(SlideInMinimize);
                        _actionQueue.QueueAction((parameter) => _appbar.Unreserve());
                        _actionQueue.QueueAction(SlideOut);
                    }
                    break;
            }
        }

        void ShowWindow()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (CanShow())
                    {
                        Log.DebugFormat("ShowWindow {0}", DockPosition);

                        _actionQueue.QueueAction(TryActivate);
                        _actionQueue.QueueAction(SlideOut);
                    }
                });
        }

        void HideWindow()
        {           
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (CanHide())
                    {
                        Log.DebugFormat("HideWindow {0}", DockPosition);
                        _actionQueue.QueueAction(SlideInMinimize);
                    }
                });
        }

        void MinimizeWindow(object parameter)
        {           
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Log.DebugFormat("MinimizeWindow, {0}", DockPosition);

                    ClearAnimations();                    

                    Position position = new Position();
                    switch (DockPosition.Selected)
                    {
                        case DockEdge.Left:
                            position.Top = ActiveScreen.Bounds.Top;
                            position.Left = ActiveScreen.Bounds.Left;
                            position.Width = 1;
                            position.Height = ActiveScreen.Bounds.Height;
                            break;
                        case DockEdge.Right:
                            position.Top = ActiveScreen.Bounds.Top;
                            position.Left = ActiveScreen.Bounds.Right;
                            position.Width = 0;
                            position.Height = ActiveScreen.Bounds.Height;
                            break;
                        case DockEdge.Top:
                            position.Top = ActiveScreen.Bounds.Top;
                            position.Left = ActiveScreen.Bounds.Left;
                            position.Width = ActiveScreen.Bounds.Width;
                            position.Height = 0;
                            break;
                        case DockEdge.Bottom:
                            position.Top = ActiveScreen.Bounds.Bottom;
                            position.Left = ActiveScreen.Bounds.Left;
                            position.Width = ActiveScreen.Bounds.Width;
                            position.Height = 0;
                            break;
                    }
                    position.Bottom = position.Top + position.Height;
                    position.Right = position.Left + position.Width;
                    
                    _appbar.CorrectPosition(ref position);

                    Log.DebugFormat("MinimizeWindow position {0}", position);

                    _mainGrid.Width = 0;
                    _mainGrid.Height = 0;
                    Width = 0;
                    Height = 0;

                    Left = position.Left;
                    Top = position.Top;
                    Width = position.Width;
                    Height = position.Height;

                    switch (DockPosition.Selected)
                    {
                        case DockEdge.Left:
                            Canvas.SetLeft(_mainGrid, 0);
                            Canvas.SetTop(_mainGrid, 0);                            
                            break;
                        case DockEdge.Right:
                            Canvas.SetLeft(_mainGrid, 0);
                            Canvas.SetTop(_mainGrid, 0);                            
                            break;
                        case DockEdge.Top:
                            Canvas.SetLeft(_mainGrid, 0);
                            Canvas.SetTop(_mainGrid, 0);                            
                            break;
                        case DockEdge.Bottom:
                            Canvas.SetLeft(_mainGrid, 0);
                            Canvas.SetTop(_mainGrid, 0);                            
                            break;
                    }

                    _mainGrid.Width = position.Width;
                    _mainGrid.Height = position.Height;

                    SwitchOrientation();

                    Log.DebugFormat("MinimzedWindow, MainGrid {0}", GetMainGridPosition());
                });
        }

        string GetMainGridPosition()
        {
            return String.Format("MainGrid Left {0}, Top {1}, Width {2}, Height {3}",
                Canvas.GetLeft(_mainGrid), Canvas.GetTop(_mainGrid), _mainGrid.Width, _mainGrid.Height);
        }

        void TryActivate(object parameter)
        {
            /*
            // Attempt to push the window to the top.
            if (!this.Activate())
            {
                Log.WarnFormat("Activate failed");                

                if (!Topmost)
                {
                    // If the activate fails use topmost to forcefully push the window on top.
                    Topmost = true;
                    Topmost = false;

                    // Another activate!
                    if (!this.Activate())
                        Log.DebugFormat("Activate still failed...");

                    // Hopefully this will push the window below the taskbar again, if it was
                    // pushed onto of the taskbar due to the Topmost switching.
                    if (!Win32Window.BringWindowToTop(_appbar.Handle))
                        Log.WarnFormat("BringWindowToTop failed");
                }
            }
             */

            // Attempt to push the window to the top.

            /*
            bool success = this.Activate();
            Log.DebugFormat("Activate, {0}", success);            

            if (!success)
            {
                // If the activate fails use topmost to forcefully push the window on top.
                if (!Topmost)
                {                   
                    Topmost = true;
                    Topmost = false;

                    // Sometimes when the window goes un-Topmost is doesn't get put 
                    // beneath the taskbar again. Hopefully this will fix it.
                    Win32Window.ChangeWindowZOrder(_appbar.Handle, HWND.TOP);
                    Win32Window.ChangeWindowZOrder(_appbar.Handle, HWND.BOTTOM);
                }

                // Another activate!
                success = this.Activate();
                Log.DebugFormat("Activate second time, {0}", success);
            }
             */

            //bool result = StealFocus(_appbar.Handle);
            //Log.DebugFormat("Win32StealFocus result {0}", result);            

            this.Activate();            

            if (_appbar.ReserveScreen)
            {
                Win32Window.ChangeWindowZOrder(_appbar.Handle, HWND.TOPMOST);
            }
            else
            {
                StealFocus(_appbar.Handle);
            }                

            //Win32Window.ChangeWindowZOrder(_appbar.Handle, HWND.TOPMOST);
            //HandleFullScreenRdpApps();
        }

        void SlideInMinimize(object parameter, ActionQueueCallback completeCallback)
        {            
            Position position = CalculateSlideInPosition();

            DockPosition dockPosition = DockPosition;
            if (parameter is DockPosition)
                dockPosition = (DockPosition)parameter;
            
            SlideInMinimize(dockPosition, completeCallback);
        }

        void SlideInMinimize(DockPosition dockPosition, ActionQueueCallback completeCallback)
        {
            Log.DebugFormat("SlideInMinimize {0}", dockPosition);

            //ClearAnimations();

            DoubleAnimation widthAnimation, heightAnimation, leftAnimation, topAnimation;
            Position position = CalculateSlideInPosition(dockPosition);

            // No point doing the sliding animation when the size of the window hasn't actually changed. 
            if (!HasPositionChanged(position))
            {
                Log.Debug("Window size has not changed significantly, cancelling sliding animation");
                completeCallback();
                return;
            }

            Storyboard storyBoard = new Storyboard();
            storyBoard.Duration = SlideDuration;
            storyBoard.Completed += (sender, e) =>
            {
                Position minimizedPosition = new Position();
                _appbar.Resize(ref minimizedPosition);

                DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        Left = position.Left;
                        Top = position.Top;
                        Width = position.Width;
                        Height = position.Height; 

                        Log.DebugFormat("SlideInMinimize Finished, MainGrid {0}", GetMainGridPosition());
                        completeCallback();
                    }));                
            };

            switch (dockPosition.Selected)
            {
                case DockEdge.Left:
                    widthAnimation = CreateGridDoubleAnimation(Grid.WidthProperty);
                    widthAnimation.To = 0;
                    storyBoard.Children.Add(widthAnimation);
                    break;
                case DockEdge.Right:                    
                    widthAnimation = CreateGridDoubleAnimation(Grid.WidthProperty);
                    widthAnimation.To = 0;
                    storyBoard.Children.Add(widthAnimation);

                    leftAnimation = CreateGridDoubleAnimation(Canvas.LeftProperty);
                    leftAnimation.To = position.Width;
                    storyBoard.Children.Add(leftAnimation);
                    break;
                case DockEdge.Top:
                    heightAnimation = CreateGridDoubleAnimation(Grid.HeightProperty);
                    heightAnimation.To = 0;
                    storyBoard.Children.Add(heightAnimation);
                    break;
                case DockEdge.Bottom:
                    heightAnimation = CreateGridDoubleAnimation(Grid.HeightProperty);
                    heightAnimation.To = 0;
                    storyBoard.Children.Add(heightAnimation);

                    topAnimation = CreateGridDoubleAnimation(Canvas.TopProperty);
                    heightAnimation.To = position.Height;
                    storyBoard.Children.Add(topAnimation);
                    break;
            }

            storyBoard.Freeze();
            storyBoard.Begin(_mainGrid);
        }

        void SlideInResize(object parameter, ActionQueueCallback callback)
        {
            Log.DebugFormat("SlideInResize {0}", DockPosition);
            Position position = CalculateSlideOutPosition();
            SlideInResize(DockPosition, callback);
        }

        void SlideInResize(DockPosition dockPosition, ActionQueueCallback completeCallback)
        {
            //ClearAnimations();

            DoubleAnimation widthAnimation, heightAnimation, leftAnimation, topAnimation;
            Position position = CalculateSlideOutPosition(dockPosition);

            // No point doing the sliding animation when the size of the window hasn't actually changed. 
            if (!HasPositionChanged(position))
            {
                Log.Debug("Window size has not changed significantly, cancelling sliding animation");
                completeCallback();
                return;
            }

            Storyboard storyBoard = new Storyboard();
            storyBoard.Duration = SlideDuration;
            storyBoard.Completed += (sender, e) =>
            {
                _appbar.Resize(ref position);

                DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                {
                    Width = position.Width;
                    Height = position.Height;
                    Left = position.Left;
                    Top = position.Top;                    

                    Log.DebugFormat("SlideInResize Finished, MainGrid {0}", GetMainGridPosition());
                    completeCallback();
                }));                
            };

            switch (dockPosition.Selected)
            {
                case DockEdge.Left:
                    widthAnimation = CreateGridDoubleAnimation(Grid.WidthProperty);
                    widthAnimation.To = position.Width;
                    storyBoard.Children.Add(widthAnimation);
                    break;
                case DockEdge.Right:
                    widthAnimation = CreateGridDoubleAnimation(Grid.WidthProperty);
                    widthAnimation.To = position.Width;
                    storyBoard.Children.Add(widthAnimation);

                    leftAnimation = CreateGridDoubleAnimation(Canvas.LeftProperty);
                    leftAnimation.From = position.Width;
                    leftAnimation.To = 0;
                    storyBoard.Children.Add(leftAnimation);
                    break;
                case DockEdge.Top:
                    heightAnimation = CreateGridDoubleAnimation(Grid.HeightProperty);
                    heightAnimation.To = position.Height;
                    storyBoard.Children.Add(heightAnimation);
                    break;
                case DockEdge.Bottom:
                    heightAnimation = CreateGridDoubleAnimation(Grid.HeightProperty);
                    heightAnimation.To = position.Height;
                    storyBoard.Children.Add(heightAnimation);

                    topAnimation = CreateGridDoubleAnimation(Canvas.TopProperty);
                    topAnimation.From = position.Height;
                    topAnimation.To = 0;
                    storyBoard.Children.Add(topAnimation);
                    break;
            }

            storyBoard.Freeze();            
            storyBoard.Begin(_mainGrid);                
        }

        void SlideOut(object parameter, ActionQueueCallback callback)
        {
            Log.DebugFormat("SlideOut {0}", DockPosition);
            Position position = CalculateSlideOutPosition();
            SlideOut(DockPosition, callback);
        }        

        void SlideOut(DockPosition dockPosition, ActionQueueCallback completeCallback)
        {
            //ClearAnimations();

            DoubleAnimation widthAnimation, heightAnimation, leftAnimation, topAnimation;
            Position position = CalculateSlideOutPosition(dockPosition);

            // No point doing the sliding animation when the size of the window hasn't actually changed. 
            if (!HasPositionChanged(position))
            {
                Log.Debug("Window size has not changed significantly, cancelling sliding animation");
                completeCallback();
                return;
            }

            Storyboard storyBoard = new Storyboard();
            storyBoard.Duration = SlideDuration;
            storyBoard.Completed += (sender, e) =>
            {               
                Log.DebugFormat("SlideOut Finished, MainGrid {0}", GetMainGridPosition());
                //HandleFullScreenRdpApps();
                completeCallback();
            };

            switch (dockPosition.Selected)
            {
                case DockEdge.Left:
                    widthAnimation = CreateGridDoubleAnimation(Grid.WidthProperty);
                    widthAnimation.To = position.Width;
                    storyBoard.Children.Add(widthAnimation);

                    _mainGrid.Height = position.Height;
                    break;
                case DockEdge.Right:
                    widthAnimation = CreateGridDoubleAnimation(Grid.WidthProperty);
                    widthAnimation.To = position.Width;
                    storyBoard.Children.Add(widthAnimation);

                    leftAnimation = CreateGridDoubleAnimation(Canvas.LeftProperty);
                    leftAnimation.From = position.Width;
                    leftAnimation.To = 0;
                    storyBoard.Children.Add(leftAnimation);

                    _mainGrid.Height = position.Height;
                    break;
                case DockEdge.Top:
                    heightAnimation = CreateGridDoubleAnimation(Grid.HeightProperty);
                    heightAnimation.To = position.Height;
                    storyBoard.Children.Add(heightAnimation);

                    _mainGrid.Width = position.Width;
                    break;
                case DockEdge.Bottom:
                    heightAnimation = CreateGridDoubleAnimation(Grid.HeightProperty);
                    heightAnimation.To = position.Height;
                    storyBoard.Children.Add(heightAnimation);

                    topAnimation = CreateGridDoubleAnimation(Canvas.TopProperty);
                    topAnimation.From = position.Height;
                    topAnimation.To = 0;
                    storyBoard.Children.Add(topAnimation);

                    _mainGrid.Width = position.Width;
                    break;
            }
            storyBoard.Freeze();

            Log.DebugFormat("Grid before appbar resize {0}", GetMainGridPosition());
            _appbar.Resize(ref position);
            Log.DebugFormat("Grid after appbar resize {0}", GetMainGridPosition());

            DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                Log.DebugFormat("Grid before window resize {0}", GetMainGridPosition());

                Top = position.Top;
                Left = position.Left;        
                Width = position.Width;
                Height = position.Height;

                Log.DebugFormat("Grid after window resize {0}", GetMainGridPosition());
                                        
                storyBoard.Begin(_mainGrid);   
            }));
        }

        void SwitchOrientation()
        {
            switch (DockPosition.Selected)
            {
                case DockEdge.Top:
                case DockEdge.Bottom:
                    _horizontalListBox.Visibility = System.Windows.Visibility.Collapsed;
                    _verticalMenu.Visibility = Visibility.Visible;                    
                    break;
                case DockEdge.Left:
                case DockEdge.Right:
                    _horizontalListBox.Visibility = System.Windows.Visibility.Visible;
                    _verticalMenu.Visibility = System.Windows.Visibility.Collapsed;                    
                    break;
            }
        }

        /// <summary>
        /// Forcing pushes the window to the top, skipping all the restrictions
        /// on Activate/SetForegroundWindow.
        /// </summary>
        public void StealFocus(IntPtr handle)
        {
            Log.DebugFormat("StealFocus");

            //TODO: I wonder if we can just do a timer to check if it hangs? Either abort the thread or even just 
            //restart the application. 

            // (a) Aborting the thread might not work since it's a native thread that is hanging

            // (b) Restarting the application should work but it is possible this would result in an infinite restart loop.
            //Compared to that, the TryActivate simply failing to work properly is probably preferable. The boolean
            //IsStealingFocus ensures that we won't ever have more than one hanging thread too.

            if (IsStealingFocus)
            {
                Log.Warn("Unable to push the dock to the foreground since it seems like we in the middle of doing it. " +
                    "This function can sometimes hang and there is nothing we can do about. If this is a problem, " +
                    "restarting the application should fix it. ");
                return;
            }            

            // Calling this asynchronously as the StealFocusTask function can hang 
            IsStealingFocus = true;
            Task task = Task.Factory.StartNew(() =>
            {
                StealFocusTask(handle);
                IsStealingFocus = false;
            });
        }

        static bool StealFocusTask(IntPtr handle)
        {
            // NOTE: The Win32.AttachThreadInput function can hang
            // See:

            // The New Old Thing - I warned you: The dangers of attaching input queues            
            // http://blogs.msdn.com/b/oldnewthing/archive/2008/08/01/8795860.aspx

            uint foregroundId = Win32Window.GetWindowThreadProcessId(Win32Window.GetForegroundWindow());
            uint currentId = Win32.GetCurrentThreadId();

            if (foregroundId != currentId)
                Win32.AttachThreadInput(foregroundId, currentId, true); // Attach

            //bool returnValue = Win32Window.SetForegroundWindow(handle);
            bool returnValue = Win32Window.BringWindowToTop(handle);
            Log.DebugFormat("BringWindowToTop, Result: {0}", returnValue);

            if (foregroundId != currentId)
                Win32.AttachThreadInput(foregroundId, currentId, false); // Deattach

            return returnValue;

            /*
            // Push window to top
            NativeMethods.SetForegroundWindow(handle);

            if (NativeMethods.GetForegroundWindow() != handle)
            {
                // That didn't work - if the foreground window belongs
                // to another thread, attach to that thread and try again
                uint foregroundId = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), IntPtr.Zero);
                uint currentId = Win32.GetCurrentThreadId();

                if (foregroundId != currentId)
                {
                    // NOTE: There might be an issue with AttachThreadInput hanging.
                    Win32.AttachThreadInput(foregroundId, currentId, true); // Attach
                    NativeMethods.SetForegroundWindow(handle);       
                    Win32.AttachThreadInput(foregroundId, currentId, false); // Deattach
                }
            }
             */
        }        

        bool CanShow()
        {
            //if (Win32Mouse.IsLeftButtonPressed && !_appbar.Dragging)
            //    return false;

            return true;
        }

        bool CanHide()
        {
            if (_appbar.ReserveScreen)
                return false;
            if (!AutoHide)
                return false; // Autohide disabled
            if (_mainGrid.Width == 0 || _mainGrid.Height == 0)
                return false; // Already minimized
            if (Mouse.DirectlyOver != null || IsMouseInBounds())
                return false; // The mouse is over the application       
            if (Win32Mouse.IsLeftButtonPressed || Win32Mouse.IsRightButtonPressed)
                return false; // Dragging something
            if (_isContextMenuOpen)
                return false;
            if (_isShellContextMenuOpen)
                return false; // Explorer menu open
            if (_isMenuOpen)
                return false;
            if (_actionQueue.IsBusy)
                return false; // Currently running a slide in or slide out animation
            if (OwnedWindows.Count > 0)
                return false; // Other application window open

            return true;
        }

        Position CalculateSlideOutPosition()
        {
            return CalculateSlideOutPosition(DockPosition);
        }        

        Position CalculateSlideOutPosition(DockPosition dockPosition)
        {
            double windowSize = CalculateWindowWidth(IconRows);
            var bounds = GetActiveScreenBounds(dockPosition);

            Position position = new Position();           

            Log.DebugFormat("CalculateSlideOut WPF bounds {0}", bounds);

            switch (dockPosition.Selected)
            {
                case DockEdge.Left:
                    position.Top = bounds.Top;
                    position.Left = bounds.Left;
                    position.Width = windowSize;
                    position.Height = bounds.Height;
                    break;
                case DockEdge.Right:
                    position.Top = bounds.Top;
                    position.Left = bounds.Right - windowSize;
                    position.Width = windowSize;
                    position.Height = bounds.Height;
                    break;
                case DockEdge.Top:
                    position.Top = bounds.Top;
                    position.Left = bounds.Left;
                    position.Width = bounds.Width;
                    position.Height = GetVerticallyDockedHeight();
                    break;
                case DockEdge.Bottom:
                    position.Top = bounds.Bottom - GetVerticallyDockedHeight();
                    position.Left = bounds.Left;
                    position.Width = bounds.Width;
                    position.Height = GetVerticallyDockedHeight();
                    break;
            }
            position.Bottom = position.Top + position.Height;
            position.Right = position.Left + position.Width;

            _appbar.CorrectPosition(ref position);            

            Log.DebugFormat("SlideOut position {0}", position);                
            return position;
        }

        Position CalculateSlideInPosition()
        {
            return CalculateSlideInPosition(DockPosition);
        }

        Position CalculateSlideInPosition(DockPosition dockPosition)
        {
            var bounds = GetActiveScreenBounds(dockPosition);

            Position position = new Position();
            switch (dockPosition.Selected)
            {
                case DockEdge.Left:
                    position.Top = this.Top;
                    position.Left = bounds.Left - this.ActualWidth;
                    position.Width = this.ActualWidth;
                    position.Height = this.ActualHeight;
                    break;
                case DockEdge.Right:
                    position.Top = this.Top;
                    position.Left = bounds.Right + this.ActualWidth;
                    position.Width = this.ActualWidth;
                    position.Height = ActiveScreen.Bounds.Height;
                    break;
                case DockEdge.Top:
                    position.Top = bounds.Top - this.ActualHeight;
                    position.Left = this.Left;
                    position.Width = this.ActualHeight;
                    position.Height = this.ActualHeight;
                    break;
                case DockEdge.Bottom:
                    position.Top = bounds.Bottom + this.ActualHeight;
                    position.Left = this.Left;
                    position.Width = this.ActualWidth;
                    position.Height = this.ActualHeight;;
                    break;
            }
            position.Bottom = position.Top + position.Height;
            position.Right = position.Left + position.Width;

            _appbar.CorrectPosition(ref position);            

            Log.DebugFormat("SlideInMinimize position {0}", position);
            return position;
        }

        DoubleAnimation CreateWindowDoubleAnimation(DependencyProperty property)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = SlideDuration;

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath(property));
            return animation;
        }

        DoubleAnimation CreateGridDoubleAnimation(DependencyProperty property)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = SlideDuration;

            Storyboard.SetTarget(animation, _mainGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath(property));
            return animation;
        }

        void ClearAnimations()
        {
            _mainGrid.BeginAnimation(Grid.HeightProperty, null);
            _mainGrid.BeginAnimation(Grid.WidthProperty, null);
            _mainGrid.BeginAnimation(Canvas.TopProperty, null);
            _mainGrid.BeginAnimation(Canvas.LeftProperty, null);            

            //this.BeginAnimation(Window.HeightProperty, null);
            //this.BeginAnimation(Window.WidthProperty, null);
            //this.BeginAnimation(Window.TopProperty, null);
            //this.BeginAnimation(Window.LeftProperty, null);            
        }

        bool IsMouseInBounds()
        {
            // Determines if the mouse is over the window.
            
            // Normally Mouse.Directly over would tell you but the window is transparent so...
            int[] position = Win32Mouse.GetMousePosition();
            Rect bounds = new Rect(Left, Top, ActualWidth, ActualHeight);
            return bounds.Contains(new System.Windows.Point(position[0], position[1]));              
        }
        
        double CalculateWindowWidth(int iconRows)
        {
            // The window needs to be big enough to be able to display all the configured rows of icons.
            double windowSize = 0;
            int size = 32;

            // TODO: At the moment the 'offset' needs to be configured for each of the rows independantly.
            // Perhaps there is a particular ratio which would work?
            switch (iconRows)
            {
                case 1:
                    windowSize = (iconRows * size) + 32;
                    break;
                case 2:
                    windowSize = (iconRows * size) + 40;
                    break;
                case 3:
                    windowSize = (iconRows * size) + 46;
                    break;
                case 4:
                    windowSize = (iconRows * size) + 52;
                    break;
                case 5:
                    windowSize = (iconRows * size) + 58;
                    break;
                default:
                    throw new NotImplementedException(iconRows.ToString());
            }
            return windowSize;
        }

        bool HasPositionChanged(Position position)
        {
            if (GetDifferenceBetween(position.Width, Width) < 10 && 
                GetDifferenceBetween(position.Height, Height) < 10 &&
                GetDifferenceBetween(position.Top, Top) < 10 &&
                GetDifferenceBetween(position.Left, Left) < 10)
            {
                return false;
            }

            return true;
        }

        double GetDifferenceBetween(double value1, double value2)
        {
            return Math.Abs(value1 - value2);
        }

        System.Drawing.Rectangle GetActiveScreenBounds(DockPosition dockPosition)
        {
            var workingArea = AdjustForWindows10DpiScaling(ActiveScreen.WorkingArea);
            var bounds = AdjustForWindows10DpiScaling(ActiveScreen.Bounds);

            //The working area of the screen is the bounds of the screen minus the 
            //start bar. If reserve screen is enabled, the dock will adjust the 
            //working area as well so we have to use the bounds

            //However, if the reserve screen is disabled we can just use on the working area
            return Configuration.ReserveScreen ? bounds : workingArea;
        }

        /// <summary>
        /// https://dzimchuk.net/best-way-to-get-dpi-value-in-wpf/
        /// </summary>
        /// <returns></returns>
        private System.Drawing.Rectangle AdjustForWindows10DpiScaling(Rectangle bounds)
        {
            var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            double dx = m.M11;
            double dy = m.M22;

            return new System.Drawing.Rectangle(
                (int)(bounds.X / dx),
                (int)(bounds.Y / dy),
                (int)(bounds.Width / dx),
                (int)(bounds.Height / dy));
        }

        int GetVerticallyDockedHeight()
        {
            int height = 40;
            var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            return (int)Math.Round(height / m.M22);
        }
    }
}
