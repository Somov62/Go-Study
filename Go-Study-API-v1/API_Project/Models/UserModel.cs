using API_Project.Areas.HelpPage.ModelDescriptions;
using DataBaseCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace API_Project.Models
{
    public class UserModel
    {
        public UserModel() { }

        public UserModel(User user)
        {
            this.Login = user.Login;
            this.Username = user.UserName;
            this.Role = user.Role.Title;
        }
        /// <summary>
        /// User's login (Email)
        /// </summary>
        [Sample("email@host.com")]
        public string Login { get; set; }

        /// <summary>
        /// User's username (may be repeated)
        /// </summary>
        [Sample("777ilyakos")]
        public string Username { get; set; }

        /// <summary>
        /// User's role. 
        /// </summary>
        [Sample("student")]
        public string Role { get; set; }
    }
}