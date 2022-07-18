using API_Project.Areas.HelpPage.ModelDescriptions;

namespace API_Project.Models.Registration
{
    public class EmailVerficationModel
    {
        public EmailVerficationModel()
        {

        }

        /// <summary>
        /// The email you need to confirm 
        /// </summary>
        [Sample("email@host.com")]
        public string Email { get; set; }
        /// <summary>
        /// Verification code
        /// </summary>
        [Sample("442621")]
        public int Code { get; set; }
    }
}