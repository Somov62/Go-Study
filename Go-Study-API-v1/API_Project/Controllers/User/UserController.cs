using API_Project.Models;
using DataBaseCore;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Project.Controllers.User
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly DbEntities _db = DbEntities.GetContext();
        private readonly LoggerLib.Logger _logger = LoggerLib.Logger.GetContext();

        /// <summary>
        /// Get user by his token
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Returns information about user as usermodel</returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(UserModel))]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetUser(string token)
        {
            ServerState.State.CountRequests++;
            _logger.CreateLog(
                new LoggerLib.LogModels.Base.BaseLogModel()
                {
                    Message = "Try Get user by token: " + token
                });
            if (string.IsNullOrEmpty(token)) return BadRequest("Incorrect token");

            var userToken = _db.UserTokens.Where(p => p.Token == token).FirstOrDefault();
            if (userToken == null) return BadRequest("Incorrect token");

            if (userToken.DateExpire < DateTime.Now) return NotFound();

            return Ok(new UserModel(userToken.User));
        }
    }
}
