using System.Windows;
using AstoundingApplications.AstoundingDock.ViewModels;
using AstoundingApplications.AstoundingDock.Utils;
using AstoundingApplications.AstoundingDock.Ui;
using AstoundingApplications.AstoundingDock.Services;
using AstoundingApplications.AstoundingDock.Models;
using GalaSoft.MvvmLight.Messaging;
using AstoundingApplications.AstoundingDock.Messages;
using System.Windows.Threading;

namespace AstoundingApplications.AstoundingDock.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DockWindow
    {
        public MainWindow()
        {       
            InitializeComponent();            
        }        
    }
}
