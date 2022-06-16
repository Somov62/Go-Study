using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Go_Study_Mobile.Views;

namespace Go_Study_Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();



            MainPage = new MainShellPage();
        }

        protected override void OnStart()
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new Page1(), true);   
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
