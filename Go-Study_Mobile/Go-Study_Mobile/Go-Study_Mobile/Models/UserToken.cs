using System;
using System.Collections.Generic;
using System.Text;

namespace Go_Study_Mobile.Models
{
    public class UserToken
    {
        public string UserLogin { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime DateExpire { get; set; }
        public string DeviceId { get; set; }
    }
}
