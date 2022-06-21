using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiService;
using DataBaseCore;
using DataBaseCore.Entities;
using Go_Study_Mobile.Models;

namespace Go_Study_Mobile.Services
{
    internal class AuthService
    {
        private UserToken _tokenData;

        public AuthService()
        {
            _tokenData = Task.Run(GetTokenFromDb).Result;
            if (_tokenData != null)
            {
                IsAuthorized = true;

            }
        }

        public bool IsAuthorized { get; private set; }
        public string Token { get; private set; }
        public UserModel User { get; private set; }
        public UserModel UpdateUserData()
        {
            if (!IsAuthorized) throw new UnauthorizedAccessException();
            UserModel user = default;
            try
            {
                user = ApiTool.Get<UserModel>("Auth");
            }
            catch (Exception ex)
            {
                Logger.LogService.GetService().WriteToLog("Try get user information", false, ex);
            }
            
            return user;
        }

        public async Task<UserToken> GetTokenFromDb()
        {
            UserToken userToken = null;

            try
            {
                var context = DbContext.GetContext();
                List<UserDataModel> data = await context.Database.Table<UserDataModel>().ToListAsync();

                var userData = data.LastOrDefault();
                if (userData == null) return null;

                if (userData.DateExpired < DateTime.Now)
                {
                    await context.Database.DeleteAsync(userData);
                }
                else userToken = new UserToken()
                {
                    Token = userData.Token,
                    RefreshToken = userData.RefreshToken,
                    DateExpire = userData.DateExpired
                };
            }
            catch (Exception ex)
            {
                Logger.LogService logService = Logger.LogService.GetService();
                logService.WriteToLog("Try read usertoken from local db", false, ex);
            }

            return userToken;
        }
    }
}
