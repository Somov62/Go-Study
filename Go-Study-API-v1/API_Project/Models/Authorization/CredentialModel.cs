using API_Project.Areas.HelpPage.ModelDescriptions;

namespace API_Project.Models.Authorization
{
    public class CredentialModel
    {
        public CredentialModel() { }

        public CredentialModel(string login, string password)
        {
            Login = login;
            Password = password;
        }

        /// <summary>
        /// Account login (Email)
        /// </summary>
        [Sample("email@host.com")]
        public string Login { get; set; }

        /// <summary>
        /// Account password (MD5 Encrypted) 
        /// </summary>
        [Sample("")]
        public string Password { get; set; }
    }
}