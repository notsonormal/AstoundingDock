using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Diagnostics.Contracts;

namespace AstoundingApplications.AstoundingDock.Services
{
    public interface IViewService : IService
    {
        void RegisterView(Type windowType, Type viewModelType);

        void OpenWindow(ViewModelBase viewModel);
        bool? OpenDialog(ViewModelBase viewModel);
    }
}
