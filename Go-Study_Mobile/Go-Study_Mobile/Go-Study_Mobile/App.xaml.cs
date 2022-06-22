using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Go_Study_Mobile.Views;
using Go_Study_Mobile.Services;

namespace Go_Study_Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            StartProgramService startProgramService = new StartProgramService();
            startProgramService.Start();

            //MainPage = new MainShellPage();
        }

        //internal StartProgramService StartingService { get;  }
        protected override void OnStart()
        {
            //Application.Current.MainPage.Navigation.PushModalAsync(new Page1(), true);
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
