using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using API_Project.Models.Authorization;
using AuthDbLib;
using DataBaseCore;

namespace API_Project.Controllers.Authorization
{
    public class AuthController : ApiController
    {
        private readonly DbEntities _db = DbEntities.GetContext();
        private readonly int _timeExpired = 300;
 
        // POST: api/Auth
        [ResponseType(typeof(AuthModel))]
        public IHttpActionResult PostUser(string login, string password, string deviceId)
        {
            if (string.IsNullOrEmpty(login)) return BadRequest("Incorrect user data");
            if (string.IsNullOrEmpty(password)) return BadRequest("Incorrect user data");
            if (string.IsNullOrEmpty(deviceId)) return BadRequest("Incorrect user data");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(login);
            if (user == null) return NotFound();
            if (!user.EmailState.IsVerificated) return BadRequest("Email is not verificated");
            if (user.Password != password) return BadRequest("Incorrect password");

            var deviceTokenInfo = user.UserTokens.Where(p => p.DeviceId == deviceId).FirstOrDefault(); 
            if (deviceTokenInfo == null)
            {
                deviceTokenInfo = new UserToken
                {
                    UserLogin = user.Login,
                    DeviceId = deviceId
                };
                _db.UserTokens.Add(deviceTokenInfo);
                _db.SaveChanges();
            
            }
            RefreshToken(deviceTokenInfo);

            try
            {
                _db.SaveChanges();
            }
            catch 
            {
                //Logger
            }
            return Ok(new AuthModel(deviceTokenInfo.Token, deviceTokenInfo.RefreshToken, deviceTokenInfo.DateExpire.Value));
        }

        [Route("api/Auth/Refresh")]
        [ResponseType(typeof(AuthModel))]
        public IHttpActionResult PostRefreshToken(string token, string refreshToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var deviceTokenInfo = _db.UserTokens.Where(p => p.Token == token).FirstOrDefault();
            if (deviceTokenInfo == null) return NotFound();
            if (deviceTokenInfo.RefreshToken != refreshToken) return BadRequest("Incorrect refreshToken");

            RefreshToken(deviceTokenInfo);

            try
            {
                _db.SaveChanges();
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok(new AuthModel(deviceTokenInfo.Token, deviceTokenInfo.RefreshToken, deviceTokenInfo.DateExpire.Value));
        }

        [Route("api/Auth/LogOut")]
        [ResponseType(typeof(AuthModel))]
        public IHttpActionResult PostLogOut(string token, string deviceId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var deviceTokenInfo = _db.UserTokens.Where(p => p.Token == token).Where(p => p.DeviceId == deviceId).FirstOrDefault();
            if (deviceTokenInfo == null) return NotFound();

            _db.UserTokens.Remove(deviceTokenInfo);

            try
            {
                _db.SaveChanges();
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok();
        }

        protected override void Dispose(bool disposing) =>  base.Dispose(disposing);

        private void RefreshToken(UserToken token)
        {
            TokenGenerator generator = new TokenGenerator();
            token.Token = generator.GenerateToken(token.User);
            token.RefreshToken = generator.GenerateToken(token.User);
            token.DateExpire = DateTime.Now.AddSeconds(_timeExpired);
        }
    }
}