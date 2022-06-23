using Android.App;
using Xamarin.Forms;
using Go_Study_Mobile.Interfaces;

[assembly: Dependency(typeof(Go_Study_Mobile.Droid.DeviceCloseApp))]

namespace Go_Study_Mobile.Droid
{
    public class DeviceCloseApp : IDeviceCloseApp
    {
        public void CloseApplication()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            //var activity = (Activity)Forms.Context;
            //activity.FinishAffinity();
        }
    }
}