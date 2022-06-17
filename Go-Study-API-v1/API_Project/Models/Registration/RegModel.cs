using DataBaseCore;
using System;

namespace API_Project.Models.Registration
{
    public class RegModel
    {
        public RegModel() { }

        public RegModel(User user)
        {
            this.Login = user.Login;
            this.UserName = user.UserName;
            this.Role = user.Role.Title;
        }
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}