using DataBaseCore;
using System;

namespace API_Project.Models
{
    public class UserModel
    {
        public UserModel() { }

        public UserModel(User user)
        {
            this.Login = user.Login;
            this.Password = user.Password;
            this.UserName = user.UserName;
            this.RoleId = user.RoleId;
            this.AccessTypeId = user.AccessTypeId;
        }
        public string Login { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public Nullable<int> RoleId { get; set; }
        public Nullable<int> AccessTypeId { get; set; }
    }
}