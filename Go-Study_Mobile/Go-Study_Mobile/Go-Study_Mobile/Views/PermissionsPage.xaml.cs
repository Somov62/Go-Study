using System;
using System.Collections.Generic;
using Go_Study_Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Go_Study_Mobile.Services.AppPermissionsManager;

namespace Go_Study_Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PermissionsPage : ContentPage
    {
        internal PermissionsPage(List<Permission> permissions, Services.StartProgramService programService)
        {
            InitializeComponent();
            BindingContext = new AppPermissionsViewModel(permissions, programService);
        }
    }
}