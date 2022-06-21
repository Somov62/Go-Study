using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Go_Study_Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Go_Study_Mobile.Services.AppPermissionsManager;

namespace Go_Study_Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PermissionsPage : ContentPage
    {
        public PermissionsPage(List<Permission> permissions)
        {
            InitializeComponent();
            BindingContext = new AppPermissionsViewModel(permissions);
        }
    }
}