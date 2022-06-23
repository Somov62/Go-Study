using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Go_Study_Mobile.Services;
using static Go_Study_Mobile.Services.AppPermissionsManager;
using System.Linq;
using System.Windows.Input;
using Go_Study_Mobile.Interfaces;

namespace Go_Study_Mobile.ViewModels
{
    public class AppPermissionsViewModel : BaseViewModel
    {
        private List<Permission> _permissions;
        internal AppPermissionsViewModel(List<Permission> permissions, Services.StartProgramService programService)
        {
            Title = "Разрешения";
            RequestPermissionCommand = new Command((object str) => RequestPermission(str.ToString()));
            CloseAppCommand = new Command(CloseApplication);
            PermissionsList = permissions;
        }

        public List<Permission> PermissionsList
        {
            get => _permissions;
            set => Set(ref _permissions, value);
        }

        public ICommand RequestPermissionCommand { get; }
        public ICommand CloseAppCommand { get; }

        private void RequestPermission(string permissionName)
        {
            Permission permission = _permissions.FirstOrDefault(p => p.Name == permissionName);
            int listIndex = _permissions.IndexOf(permission);

            AppPermissionsManager manager = new AppPermissionsManager();
            PermissionsList[listIndex] = manager.RequestPermission(permission);
        }

        private void CloseApplication()
        {
            IDeviceCloseApp closer = DependencyService.Get<IDeviceCloseApp>();
            closer?.CloseApplication();
        }
    }
}
