using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Logger;

[assembly: Dependency(typeof(Go_Study_Mobile.Droid.DeviceLogService))]

namespace Go_Study_Mobile.Droid
{
    public class DeviceLogService : IDeviceLogService
    {
        public string PathToLogFoldFolder
        {
            get
            {
                if (Android.OS.Environment.IsExternalStorageEmulated)
                {
                    return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                }
                return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            }
        }
    }
}