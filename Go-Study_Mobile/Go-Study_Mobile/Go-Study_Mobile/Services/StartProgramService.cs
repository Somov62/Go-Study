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

        }

        public void CheckStartCapability()
        {

        }

        public void Start()
        {
            SetDeviceFontSize();

            AppPermissionsManager permissionsManager = new AppPermissionsManager();
            if (!permissionsManager.IsAllGranted)
            {
                Application.Current.MainPage = new PermissionsPage(permissionsManager.GetPermissionsList(), this);
                return;
            }

            LaunchProgram();
        }
        
        private void LaunchProgram()
        {
            _ = new Logger.LogService(DependencyService.Get<Logger.IDeviceLogService>());


            AuthService authService = new AuthService();

            if (!authService.IsAuthorized)
            {
                Application.Current.MainPage = new AuthPage();
            }
        }

        private void SetDeviceFontSize()
        {
            var screeninfo  =  Xamarin.Essentials.DeviceDisplay.MainDisplayInfo;
            Application.Current.Resources["ScreenResolution"] = Math.Sqrt(Math.Pow(screeninfo.Height, 2) + Math.Pow(screeninfo.Width, 2)) * screeninfo.Density;
        }
    }
}
