using API_Project.Areas.HelpPage.ModelDescriptions;
using DataBaseCore;

namespace API_Project.Models.Registration
{
    public class RegModel : UserModel
    {
        public RegModel() : base () { }

        public RegModel(User user)
        {
            this.Login = user.Login;
            this.Username = user.UserName;
            this.Role = user.Role.Title;
        }

        /// <summary>
        /// Account password (MD5 Encrypted) 
        /// </summary>
        [Sample("")]
        public string Password { get; set; }
        
    }
}