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
                Name = "Доступ к внутреннему хранилищу",
                Type = Permissions.ExternalStorageWrite,
                IsGranted = null,
                Description = "Доступ к внутреннему хранилищу"
            },
            new Permission()
            {
                Name = "Доступ к внутреннему хранилищу",
                Type = Permissions.ExternalStorageWrite,
                IsGranted = null,
                Description = "Доступ к внутреннему хранилищу"
            }
        };

        public AppPermissionsManager()
        {
            CheckPermissions();
        }
        public bool IsAllGranted => ReturnPermissionsStatus();
        private bool ReturnPermissionsStatus()
        {
            foreach (var item in GetPermissionsList())
            {
                if (item.IsGranted != true) return false;
            }
            return true;
        }

        private void CheckPermissions()
        {
            _permissions[0].IsGranted = CheckExternalStorageWrite();
        }

        public List<Permission> GetPermissionsList()
        {
            CheckPermissions();
            return _permissions;
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
            public string Name { get; set; }
            public Permissions Type{ get; set; }
            public bool? IsGranted { get; set; }
            public string Description { get; set; }
        }

        public enum Permissions
        {
            ExternalStorageWrite
        }
    }
}
