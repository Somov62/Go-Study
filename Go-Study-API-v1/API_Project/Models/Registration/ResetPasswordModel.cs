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

        /// <summary>
        /// Verification code
        /// </summary>
        [Sample("b270eb1a-10e1-4e6a-9113-ba8f7758b4fd")]
        public System.Guid SessionId { get; set; }
    }
}