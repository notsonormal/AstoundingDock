using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AstoundingApplications.AstoundingDock.Services
{
    interface IOpenFileService : IService
    {
        bool? ShowDialog();
        bool? ShowDialog(Window owner);
        string OpenedFile { get; }
        string Filter { get; set; }
        string InitialDirectory { get; set; }
    }
}
