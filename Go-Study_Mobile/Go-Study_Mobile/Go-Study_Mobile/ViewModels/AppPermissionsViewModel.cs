using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Go_Study_Mobile.Services;
using static Go_Study_Mobile.Services.AppPermissionsManager;
using System.Linq;
using System.Windows.Input;
using Go_Study_Mobile.Interfaces;
using System.Threading.Tasks;
using Xamarin.Essentials;

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
            set => Set(ref _permissions, value, nameof(PermissionsList));
        }

        public ICommand RequestPermissionCommand { get; }
        public ICommand CloseAppCommand { get; }

        private async void RequestPermission(string permissionName)
        {
            Permission permission = _permissions.FirstOrDefault(p => p.Name == permissionName);
            int listIndex = _permissions.IndexOf(permission);

            AppPermissionsManager manager = new AppPermissionsManager();
            var list = new List<Permission>(PermissionsList);
            list[listIndex] = await manager.RequestPermission(permission);
            PermissionsList = list;
        }

        private void CloseApplication()
        {
            IDeviceCloseApp closer = DependencyService.Get<IDeviceCloseApp>();
            closer?.CloseApplication();
        }
    }
}
