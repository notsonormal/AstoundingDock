using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using AstoundingApplications.AstoundingDock.Extensions;

namespace AstoundingApplications.AstoundingDock.Services
{
    class OpenFileService : IOpenFileService
    {
        public bool? ShowDialog()
        {
            // TODO: I don't actually use the owner for OpenFileDialog?
            return ShowDialog(Application.Current.MainWindow);
        }

        public bool? ShowDialog(Window owner)
        {
            OpenedFile = null;
            OpenFileDialog ofd = new OpenFileDialog();

            // Set OpenFileDialog parameters.
            if (!String.IsNullOrEmpty(Title))
                ofd.Title = Title;
            if (!String.IsNullOrEmpty(Filter))
                ofd.Filter = Filter;
            if (!String.IsNullOrEmpty(InitialDirectory))
                ofd.InitialDirectory = InitialDirectory;
            if (!String.IsNullOrEmpty(DefaultExt))
                ofd.DefaultExt = DefaultExt;

            // Opened dialog and store selected file name.
            bool? result = ofd.ShowDialog(owner);
            OpenedFile = ofd.FileName;

            // Clear the OpenFileDialog parameters values.
            Title = null;
            Filter = null;
            InitialDirectory = null;
            DefaultExt = null;

            // Return result.
            return result;
        }

        public string OpenedFile { get; private set; }
        public string Title { get; set; }
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }
        public string DefaultExt { get; set; }
    }
}
