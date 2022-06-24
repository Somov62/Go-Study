using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Go_Study_Mobile.Services
{
    public class AppPermissionsManager
    {
        private readonly List<Permission> _permissions = new List<Permission>()
        {
            new Permission()
            {
                Name = "Внутреннее хранилище",
                Type = Permissions.ExternalStorageWrite,
                IsGranted = null,
                Description = "Доступ к внутреннему хранилищу, для сохранения данных приложения"
            },
            new Permission()
            {
                Name = "Камера",
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
        
        internal Permission RequestPermission(Permission permission)
        {
            switch (permission.Type)
            {
                case Permissions.ExternalStorageWrite:
                    //permission.IsGranted = await RequestExternalStorageWrite1();
                    return permission;
                default:
                    return permission;
            }
        }
        

        public List<Permission> GetPermissionsList()
        {
            CheckPermissions();
            return _permissions;
        }
        private void CheckPermissions()
        {
            _permissions[0].IsGranted = CheckExternalStorageWrite();
        }
        private bool ReturnPermissionsStatus()
        {
            foreach (var item in GetPermissionsList())
            {
                if (item.IsGranted != true) return false;
            }
            return true;
        }
        private bool CheckExternalStorageWrite()
        {
            var status = Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.StorageWrite>().Result;

            if (status == PermissionStatus.Granted)
                return true;
            return false;
        }

        private async Task<bool?> RequestExternalStorageWrite1()
        {
            var status = await RequestExternalStorageWrite();
            return status;
        }
        private async Task<bool?> RequestExternalStorageWrite()
        {
            var status = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();

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
