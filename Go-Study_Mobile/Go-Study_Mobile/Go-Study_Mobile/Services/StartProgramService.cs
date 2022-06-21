using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Go_Study_Mobile.Views;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Go_Study_Mobile.Services
{
    internal class StartProgramService
    {
        public StartProgramService()
        {
            CheckAndRequestLocationPermission();
            _ = new Logger.LogService(DependencyService.Get<Logger.IDeviceLogService>());
        }

        public void Start()
        {
            AuthService authService = new AuthService();

            if (!authService.IsAuthorized)
            {
                Application.Current.MainPage = new AuthPage();
            }
        }
        
    }
}
