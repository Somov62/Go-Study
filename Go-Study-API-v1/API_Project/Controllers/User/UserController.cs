using API_Project.Models;
using DataBaseCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Project.Controllers.User
{
    [RoutePrefix("user")]
    public class UserController : ApiController
    {
        private readonly DbEntities _db = DbEntities.GetContext();

        /// <summary>
        /// Get user by his token
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Returns information about user as usermodel</returns>
        [HttpGet]
        [ResponseType(typeof(UserModel))]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetUser(string token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest("Incorrect token");

            var userToken = _db.UserTokens.Where(p => p.Token == token).FirstOrDefault();
            if (userToken == null) return NotFound();

            if (userToken.DateExpire < DateTime.Now) return NotFound();

            return Ok(new UserModel(userToken.User));
        }
    }
}
