using System;
using System.Collections.Generic;
using Xamarin.Forms;
using static Go_Study_Mobile.Services.AppPermissionsManager;

namespace Go_Study_Mobile.ViewModels
{
    public class AppPermissionsViewModel : BaseViewModel
    {
        private List<Permission> _permissions;
        internal AppPermissionsViewModel(List<Permission> permissions, Services.StartProgramService programService)
        {
            Title = "Разрешения";
            PermissionsList = permissions;
        }

        public List<Permission> PermissionsList
        {
            get => _permissions;
            set => Set(ref _permissions, value);
        }

        public Command MyProperty { get; set; }
    }
}
