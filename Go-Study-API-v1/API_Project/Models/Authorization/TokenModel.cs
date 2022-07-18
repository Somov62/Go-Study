using API_Project.Areas.HelpPage.ModelDescriptions;
using System;

namespace API_Project.Models.Authorization
{
    public sealed class TokenModel
    {
        public TokenModel() { }
        public TokenModel(string token, string refreshToken, DateTime dateExpired)
        {
            Token = token;
            RefreshToken = refreshToken;
            DateExpired = dateExpired;
        }

        /// <summary>
        /// Session token representing access to Go Study system
        /// </summary>
        [Sample("")]
        public string Token { get; set; }

        /// <summary>
        /// Needed to update the token without re-authorization
        /// </summary>
        [Sample("")]
        public string RefreshToken{ get; set; }

        /// <summary>
        /// Date expired token. Refresh the token so as not to lose access
        /// </summary>
        [Sample("2022-07-18T14:45:27.681474+03:00")]
        public DateTime DateExpired { get; set; }
    }
}