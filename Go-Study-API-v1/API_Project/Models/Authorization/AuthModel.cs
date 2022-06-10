using System;

namespace API_Project.Models.Authorization
{
    public class AuthModel
    {
        public AuthModel(string token, string refreshToken, DateTime dateExpired)
        {
            Token = token;
            RefreshToken = refreshToken;
            DateExpired = dateExpired;
        }

        public string Token { get; set; }
        public string RefreshToken{ get; set; }
        public DateTime DateExpired { get; set; }
    }
}