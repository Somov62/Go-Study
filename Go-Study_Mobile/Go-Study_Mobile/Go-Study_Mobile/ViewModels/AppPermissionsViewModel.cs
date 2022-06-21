using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using static Go_Study_Mobile.Services.AppPermissionsManager;

namespace Go_Study_Mobile.ViewModels
{
    public class AppPermissionsViewModel : BaseViewModel
    {
        public AppPermissionsViewModel(List<Permission> permissions)
        {
            Title = "Разрешения";
        }

        public Command MyProperty { get; set; }
    }
}
