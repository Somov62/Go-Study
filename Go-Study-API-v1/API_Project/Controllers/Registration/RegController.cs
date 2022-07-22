using API_Project.Extensions;
using API_Project.Models;
using API_Project.Models.Registration;
using DataBaseCore;
using LoggerLib;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Project.Controllers.Registration
{
    [RoutePrefix("api/reg")]
    public class RegController : ApiController
    {
        private readonly Logger _logger = Logger.GetContext();
        private readonly DbEntities _db = DbEntities.GetContext();

        #region Registration new user
        /// <summary>
        /// Method for registration new account
        /// </summary>
        /// <param name="user"></param>
        /// <returns>If success, return user information</returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostUser([FromBody] RegModel user)
        {
            #region Validating
            if (string.IsNullOrEmpty(user.Login)) return BadRequest("Incorrect user data");
            if (string.IsNullOrEmpty(user.Password)) return BadRequest("Incorrect user data");
            if (string.IsNullOrEmpty(user.Username)) return BadRequest("Incorrect user data");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            DataValidator.DataValidator validator = new DataValidator.DataValidator();
            if (!validator.ValidateEmail(user.Login)) return BadRequest("Incorrect user data");
            if (!validator.ValidateMD5(user.Password)) return BadRequest("Incorrect user data");

            if (_db.Users.Find(user.Login) != null) return BadRequest("This email already registration");

            if (_db.Roles.Where(prop => prop.Title == user.Role).FirstOrDefault() == null) return BadRequest("Incorrect user data");
            #endregion

            DataBaseCore.User newUser = new DataBaseCore.User()
            {
                UserName = user.Username,
                Login = user.Login,
                Password = user.Password,
                RoleId = _db.Roles.Where(prop => prop.Title == user.Role).FirstOrDefault().Id,
                AccessTypeId = 1
            };

            Random rnd = new Random();

            DataBaseCore.EmailState emailState = new DataBaseCore.EmailState()
            {
                UserLogin = user.Login,
                VerificationCode = rnd.Next(23981, 100000),
                DateSentCode = DateTime.Now,
                IsVerificated = false
            };

            try
            {
                _db.Users.Add(newUser);
                _db.EmailStates.Add(emailState);
                _db.SaveChanges();
                bool isSendSuccess = SendVerificationCode(newUser.Login, emailState.VerificationCode);
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
                        Message = $"Ошибка при сохранении нового аккаунта.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
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
                        Message = $"Успешная регистрация.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
            #endregion
            return Ok(new UserModel(newUser));
        }

        /// <summary>
        /// Use to confirm email after registration
        /// </summary>
        /// <param name="verficationData">Account email and verification code from email message</param>
        /// <returns>If success, return user information</returns>
        [HttpPost]
        [Route("confirmEmail")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostCheckVerificationCode([FromBody] EmailVerficationModel verficationData)
        {
            #region Validating
            if (string.IsNullOrEmpty(verficationData.Email)) return BadRequest("Incorrect email");
            if (verficationData.Code < 23981 || verficationData.Code > 100000) return BadRequest("Incorrect verification code");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(verficationData.Email);
            if (user == null) return NotFound();
            #endregion

            if (user.EmailState.IsVerificated) return BadRequest("Email already verificated");

            if (user.EmailState.DateSentCode.AddSeconds(600) < DateTime.Now) return BadRequest("Verificated code expired");
            if (user.EmailState.VerificationCode != verficationData.Code) return BadRequest("Incorrect verification code");

            user.EmailState.IsVerificated = true;
            user.EmailState.VerificationCode = -1;

            try
            {
                _db.SaveChanges();
                bool isSendSuccess = SendVerificationSuccess(user.Login, user.UserName);
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
                        Message = $"Ошибка при сохранении подтверждения почты.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
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
                        Message = $"Успешное подтверждение электронной почты.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
            #endregion
            return Ok(new UserModel(user));
        }
        #endregion

        #region Reset password
        /// <summary>
        /// Allows you to reset the password, sends the code to the account email
        /// </summary>
        /// <param name="login">Account email</param>
        /// <returns>Returns session number for confirm youself in second part reset password</returns>
        [HttpPost]
        [Route("resetPassword")]
        [ResponseType(typeof(string))]
        public IHttpActionResult PostResetPassword([FromBody] string login)
        {
            #region Validating
            if (string.IsNullOrEmpty(login)) return BadRequest("Incorrect email");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(login);
            if (user == null) return NotFound();
            
            if (!user.EmailState.IsVerificated && _db.ResetPasswordSessions.Where(p=> p.UserLogin == user.Login).Count() == 0)
            {
                #region log
                _logger.CreateLog(
                    new LoggerLib.LogModels.Base.BaseLogModel()
                    {
                        Type = LogType.Warning,
                        CurrentMethod = System.Reflection.MethodBase.GetCurrentMethod().Name,
                        Message = $"Попытка сбросить пароль на аккаунте с неподтвержденной почтой.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
                #endregion
                return BadRequest("Please, finish registration");
            }
            #endregion

            Random rnd = new Random();
            user.EmailState.VerificationCode = rnd.Next(23981, 100000);
            user.EmailState.DateSentCode = DateTime.Now;

            _db.ResetPasswordSessions.RemoveRange(_db.ResetPasswordSessions.Where(p => p.UserLogin == user.Login));

            ResetPasswordSession passwordSession = new ResetPasswordSession()
            {
                Id = Guid.NewGuid(),
                UserLogin = user.Login,
                CancelCode = rnd.Next(2000123, 9999999).ToString()
            };

            try
            {
                _db.ResetPasswordSessions.Add(passwordSession);
                _db.SaveChanges();
                string key = $"{passwordSession.Id}{passwordSession.CancelCode}";
                string cancelRequestPath = $"{Request.RequestUri.AbsoluteUri}/{key}/cancel";
                bool isSendSuccess = SendResetPasswordCode(user.Login, user.EmailState.VerificationCode, cancelRequestPath);
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
                        Message = $"Ошибка при попытке сброса пароля.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
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
                        Message = $"Успешный 1 этап сброса пароля.\nLogin: {user.Login}\nPasswordSession: {passwordSession.Id}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
            #endregion
            return Ok(passwordSession.Id);
        }

        /// <summary>
        /// Use to finish reset password
        /// </summary>
        /// <param name="resetPasswordData">Model email, new password, verification code</param>
        /// <returns>If success, return user information</returns>
        [HttpPost]
        [Route("confirmResetPassword")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostCheckResetPasswordCode([FromBody] ResetPasswordModel resetPasswordData)
        {
            #region Validating
            if (string.IsNullOrEmpty(resetPasswordData.Login)) return BadRequest("Incorrect email");
            if (string.IsNullOrEmpty(resetPasswordData.Password)) return BadRequest("Incorrect password");
            
            DataValidator.DataValidator validator = new DataValidator.DataValidator();
            if (!validator.ValidateMD5(resetPasswordData.Password)) return BadRequest("Incorrect password");

            if (resetPasswordData.Code < 23981 || resetPasswordData.Code > 100000) return BadRequest("Incorrect verification code");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(resetPasswordData.Login);
            if (user == null) return NotFound();
            var session = _db.ResetPasswordSessions.Find(resetPasswordData.SessionId);
            if (session == null) return NotFound();

            if (!user.EmailState.IsVerificated)
            {
                return BadRequest("Reset password procedure has been rejected");
            }
            #endregion

            if (user.EmailState.DateSentCode.AddSeconds(600) < DateTime.Now) return BadRequest("Verificated code expired");
            if (user.EmailState.VerificationCode != resetPasswordData.Code) return BadRequest("Incorrect verification code");

            user.EmailState.VerificationCode = -1;

            user.Password = resetPasswordData.Password;

            user.AccessTypeId = 1; 

            try
            {
                _db.SaveChanges();
                string key = $"{session.Id}{session.CancelCode}";
                string freezeRequestPath = $"{Request.RequestUri.AbsoluteUri}/{key}/freeze";
                bool isSendSuccess = SendResetPasswordSuccess(user.Login, user.UserName, freezeRequestPath);
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
                        Message = $"Ошибка при сохранении сброшенного пароля.\nLogin: {user.Login}\nPasswordSession: {resetPasswordData.SessionId}\nIp-Address: {Request.GetClientIpAddress()}"
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
                        Message = $"Успешный 2 этап сброса пароля.\nLogin: {user.Login}\nPasswordSession: {resetPasswordData.SessionId}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
            #endregion
            return Ok(new UserModel(user));
        }
        #endregion


        [HttpGet]
        [Route("resetPassword/{key}/cancel")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult CancelResetPasswordSession(string key)
        {
            #region Validating
            if (key.Length < 8) return BadRequest();

            string keyGuid = key.Substring(0, key.Length - 7);
            string keyCancelCode = key.Substring(key.Length - 7);

            var session = _db.ResetPasswordSessions.Find(Guid.Parse(keyGuid));

            if (session == null) return Ok("Password already changed, check you email");

            if (session.CancelCode != keyCancelCode) return StatusCode(System.Net.HttpStatusCode.Forbidden);
            #endregion

            _db.ResetPasswordSessions.Remove(session);

            var user = _db.Users.Find(session.UserLogin);
            if (user != null) user.EmailState.VerificationCode = -1;

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
                        Message = $"Ошибка при сохранении отмены сброса пароля.\nLogin: {user.Login}\nPasswordSession: {keyGuid}\nIp-Address: {Request.GetClientIpAddress()}"
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
                        Message = $"Сессия сброса пароля отменена.\nLogin: {user.Login}\nPasswordSession: {keyGuid}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
            #endregion
            return Ok("Session successfuly deleted, account is safe");
        }

        [HttpGet]
        [Route("confirmResetPassword/{key}/freeze")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult FreezeAccount(string key)
        {
            #region Validating
            if (key.Length < 8) return BadRequest();

            string keyGuid = key.Substring(0, key.Length - 7);
            string keyCancelCode = key.Substring(key.Length - 7);

            var session = _db.ResetPasswordSessions.Find(Guid.Parse(keyGuid));

            if (session == null) return Ok("Password alreasy changed, check you email");

            if (session.CancelCode != keyCancelCode) return StatusCode(System.Net.HttpStatusCode.Forbidden);
            #endregion

            var user = _db.Users.Find(session.UserLogin);
            if (user == null) return Ok("Account not found");
            
            user.EmailState.VerificationCode = -1;
            user.AccessTypeId = _db.AccessTypes.Where(p => p.Type == "Blocked").First().Id;

            _db.ResetPasswordSessions.RemoveRange(_db.ResetPasswordSessions.Where(p => p.UserLogin == user.Login));
            _db.UserTokens.RemoveRange(_db.UserTokens.Where(p => p.UserLogin == user.Login));

            try 
            { 
                _db.SaveChanges();
                bool isSendSuccess = SendAccountFreeze(user.Login, user.UserName);
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
                        Message = $"Ошибка при сохранении заморозки аккаунта.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
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
                        Message = $"Аккаунт заморожен.\nLogin: {user.Login}\nIp-Address: {Request.GetClientIpAddress()}"
                    });
            #endregion
            return Ok("Account successfuly freezed");
        }


        [HttpGet]
        [Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult Getlol()
        {
            return Ok(Request.RequestUri.AbsoluteUri);
        }

        private bool SendVerificationCode(string email, int code)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.ConfirmEmailMessage(email, code));
        }

        private bool SendVerificationSuccess(string email, string username)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.SuccessConfirmEmailMessage(email, username));
        }

        private bool SendResetPasswordCode(string email, int code, string cancelRequestPath)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.ConfirmResetPasswordMessage(email, code, cancelRequestPath));
        }

        private bool SendResetPasswordSuccess(string email, string username, string freezeRequestPath)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.SuccessResetPasswordMessage(email, username, freezeRequestPath));
        }

        private bool SendAccountFreeze(string email, string username)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.FreezeAccountMessage(email, username));
        }
        protected override void Dispose(bool disposing) => base.Dispose(disposing);

    }
}