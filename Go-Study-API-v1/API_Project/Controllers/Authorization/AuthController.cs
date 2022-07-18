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
    [RoutePrefix("auth")]
    public class AuthController : ApiController
    {
        private readonly DbEntities _db = DbEntities.GetContext();
        private readonly int _timeExpired = 300;

        /// <summary>
        /// Authorization user in system. Getting a token.
        /// </summary>
        /// <param name="credentials">Login and encrypted password</param>
        /// <returns>Returns token data for further connection to the server</returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(TokenModel))]
        public IHttpActionResult PostUser([FromBody] CredentialModel credentials)
        {
            #region Validating
            if (string.IsNullOrEmpty(credentials.Login)) return BadRequest("Incorrect user data");
            if (string.IsNullOrEmpty(credentials.Password)) return BadRequest("Incorrect user data");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(credentials.Login);
            if (user == null) return NotFound();
            if (!user.EmailState.IsVerificated) return BadRequest("Email is not verificated");
            if (user.Password != credentials.Password) return BadRequest("Incorrect password");
            #endregion

            var sessionToken = CreateSession(user);

            try { _db.SaveChanges(); }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok(new TokenModel(sessionToken.Token, sessionToken.RefreshToken, sessionToken.DateExpire));
        }

        /// <summary>
        /// Refresh the token so as not to lose access
        /// </summary>
        /// <param name="tokenInfo">Token model</param>
        /// <returns>Returns new activated token model</returns>
        [HttpPost]
        [Route("refresh")]
        [ResponseType(typeof(TokenModel))]
        public IHttpActionResult PostRefreshToken([FromBody] TokenModel tokenInfo)
        {
            #region Validating
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sessionInfo = _db.UserTokens.Where(p => p.Token == tokenInfo.Token).FirstOrDefault();
            if (sessionInfo == null) return NotFound();
            if (sessionInfo.RefreshToken != tokenInfo.RefreshToken) return BadRequest("Incorrect refreshToken");
            #endregion

            RefreshToken(sessionInfo);

            try { _db.SaveChanges(); }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok(new TokenModel(sessionInfo.Token, sessionInfo.RefreshToken, sessionInfo.DateExpire));
        }

        /// <summary>
        /// Logout Go Study system
        /// </summary>
        /// <param name="token">Account Token</param>
        /// <returns>Returns http action result code</returns>
        [HttpPost]
        [Route("logout")]
        [ResponseType(typeof(TokenModel))]
        public IHttpActionResult PostLogOut(string token)
        {
            #region Validating
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var sessionInfo = _db.UserTokens.Where(p => p.Token == token).FirstOrDefault();
            if (sessionInfo == null) return NotFound();
            #endregion

            _db.UserTokens.Remove(sessionInfo);

            try { _db.SaveChanges(); }
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

        private UserToken CreateSession(DataBaseCore.User client)
        {
            var sessionToken = new UserToken
            {
                UserLogin = client.Login,
            };
            RefreshToken(sessionToken);

            _db.UserTokens.Add(sessionToken);
            _db.SaveChanges();

            return sessionToken;
        }
    }
}