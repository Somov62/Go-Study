using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Go_Study_Mobile.Services
{
    public class AppPermissionsManager
    {
        private readonly List<Permission> _permissions = new List<Permission>()
        {
            new Permission()
            {
                Name = Permissions.ExternalStorageWrite,
                IsGranted = null
            }
        };

        public AppPermissionsManager()
        {
            CheckPermissions();
        }
        public List<Permission> PermissionsList => _permissions;

        private void CheckPermissions()
        {
            foreach (Permission item in _permissions)
            {
                if (item.IsGranted == true) continue;
                switch (item.Name)
                {
                    case Permissions.ExternalStorageWrite:
                        item.IsGranted = RequestExternalStorageWrite();
                        break;
                }
            }
        }


        private bool CheckExternalStorageWrite()
        {
            var status = Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.StorageWrite>().Result;

            if (status == PermissionStatus.Granted)
                return true;
            return false;
        }

        private bool? RequestExternalStorageWrite()
        {
            var status = Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>().Result;

            if (status == PermissionStatus.Granted) return true;
            if (status == PermissionStatus.Disabled) return false;
            return null;
        }



    

        public class Permission
        {
            public Permissions Name { get; set; }
            public bool? IsGranted { get; set; }

            public string MyProperty { get; set; }
        }

        public enum Permissions
        {
            ExternalStorageWrite
        }
    }
}
