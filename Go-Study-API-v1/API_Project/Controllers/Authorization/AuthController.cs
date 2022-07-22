using System;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using API_Project.Models.Authorization;
using AuthDbLib;
using DataBaseCore;
using API_Project.Extensions;
using LoggerLib;

namespace API_Project.Controllers.Authorization
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly Logger _logger = Logger.GetContext();
        private readonly DbEntities _db = DbEntities.GetContext();
        private readonly int _timeExpired = 300;

        #region API methods
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

            if (user.AccessTypeId == _db.AccessTypes.Where(p => p.Type == "Blocked").First().Id)
            {
                #region log
                _logger.CreateLog(
                    new LoggerLib.LogModels.Base.BaseLogModel()
                    {
                        Type = LogType.Warning,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Попытка войти в заблокированный аккаунт.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
                #endregion
                return BadRequest("Account blocked. Please, change password");
            }

            if (!user.EmailState.IsVerificated) return BadRequest("Email is not verificated");
            if (user.Password != credentials.Password) return BadRequest("Incorrect password");
            #endregion

            UserToken sessionToken = CreateSession(user);

            try
            {
                _db.SaveChanges();

                Guid recordId = _db.UserTokens.Where(p => p.Token == sessionToken.Token).First().Id;
                string banUrl = $"{Request.RequestUri.AbsoluteUri}/{recordId}{sessionToken.Token.Substring(1, 5)}/logout";
                bool isSendSuccess = SendLoginMessage(user.Login, banUrl, Request.GetClientIpAddress());
                if (!isSendSuccess) return BadRequest("The limit of sent messages has been exceeded");
            }
            catch (Exception ex)
            {
                #region log
                _logger.CreateLog(
                    new LoggerLib.LogModels.ExceptionLogModel()
                    {
                        Type = LogType.Error,
                        Exception = ex,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Ошибка при сохранении авторизации.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
                #endregion
                return InternalServerError(new Exception("Something went wrong"));
            }
            #region log
            _logger.CreateLog(
                    new LoggerLib.LogModels.Base.BaseLogModel()
                    {
                        Type = LogType.Trace,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Успешная авторизация.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
            #endregion
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
            if (sessionInfo.RefreshToken != tokenInfo.RefreshToken)
            {
                #region log
                _logger.CreateLog(
                    new LoggerLib.LogModels.Base.BaseLogModel()
                    {
                        Type = LogType.Warning,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Обновление токена отклонено. Совпадение токенов, но несовпадение токенов для обновления.\nLogin: {sessionInfo.UserLogin}\nSession: {sessionInfo.Token}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
                #endregion
                return BadRequest("Incorrect refreshToken");
            }
            #endregion

            RefreshToken(sessionInfo);

            try { _db.SaveChanges(); }
            catch (Exception ex)
            {
                #region log
                _logger.CreateLog(
                    new LoggerLib.LogModels.ExceptionLogModel()
                    {
                        Type = LogType.Error,
                        Exception = ex,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Ошибка при сохранении обновленного токена.\nLogin: {sessionInfo.UserLogin}\nToken: {tokenInfo.Token}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
                #endregion
                return InternalServerError(new Exception("Something went wrong"));
            }
            #region log
            _logger.CreateLog(
                    new LoggerLib.LogModels.Base.BaseLogModel()
                    {
                        Type = LogType.Trace,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Успешное обновление токена.\nLogin: {sessionInfo.UserLogin}"
                    });
            #endregion
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
            catch (Exception ex)
            {
                #region log
                _logger.CreateLog(
                    new LoggerLib.LogModels.ExceptionLogModel()
                    {
                        Type = LogType.Error,
                        Exception = ex,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Ошибка при сохранении выхода из аккаунта.\nLogin: {sessionInfo.UserLogin}\nSession: {sessionInfo.Token}"
                    });
                #endregion
                return InternalServerError(new Exception("Something went wrong"));
            }
            #region log
            _logger.CreateLog(
                    new LoggerLib.LogModels.Base.BaseLogModel()
                    {
                        Type = LogType.Trace,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Успешный выход из аккаунта.\nLogin: {sessionInfo.UserLogin}\nSession: {sessionInfo.Token}"
                    });
            #endregion
            return Ok();
        }

        [HttpGet]
        [Route("{id}/logout")]
        [ResponseType(typeof(TokenModel))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult BanLogOut(string id)
        {
            #region Validating
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var guid = Guid.Parse(id.Substring(0, id.Length - 5));
            var sessionInfo = _db.UserTokens.Find(guid);
            if (sessionInfo == null) return NotFound();

            if (sessionInfo.Token.Substring(1, 5) != id.Substring(id.Length - 5)) return BadRequest();
            #endregion

            _db.UserTokens.Remove(sessionInfo);

            try { _db.SaveChanges(); }
            catch (Exception ex)
            {
                #region log
                _logger.CreateLog(
                    new LoggerLib.LogModels.ExceptionLogModel()
                    {
                        Type = LogType.Error,
                        Exception = ex,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Ошибка при принудительном завершении сессии.\nLogin: {sessionInfo.UserLogin}\nSession: {sessionInfo.Token}"
                    });
                #endregion
                return InternalServerError(new Exception("Something went wrong"));
            }
            #region log
            _logger.CreateLog(
                    new LoggerLib.LogModels.Base.BaseLogModel()
                    {
                        Type = LogType.Trace,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Заблокированная сессия.\nLogin: {sessionInfo.UserLogin}\nSession: {sessionInfo.Token}"
                    });
            #endregion
            return Ok("Suspicious session successfully terminated");
        }

        #endregion

        #region Supportive methods
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
            UserToken sessionToken = new UserToken
            {
                Id = Guid.NewGuid(),
                UserLogin = client.Login,
                User = client
            };

            RefreshToken(sessionToken);
            sessionToken.User = null;
            _db.UserTokens.Add(sessionToken);
            _db.SaveChanges();
            return sessionToken;
        }

        private bool SendLoginMessage(string email, string resetAddress, string ipAddress)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.LoginMessage(email, resetAddress, ipAddress));
        }
        #endregion
    }
}