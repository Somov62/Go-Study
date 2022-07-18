using API_Project.Areas.HelpPage.ModelDescriptions;
using API_Project.Models.Authorization;

namespace API_Project.Models.Registration
{
    public class ResetPasswordModel : CredentialModel
    {
        public ResetPasswordModel() { }

        /// <summary>
        /// Verification code
        /// </summary>
        [Sample("442621")]
        public int Code { get; set; }
    }
}