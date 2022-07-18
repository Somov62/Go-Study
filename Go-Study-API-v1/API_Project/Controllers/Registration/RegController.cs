using API_Project.Models;
using API_Project.Models.Registration;
using DataBaseCore;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using API_Project.Extensions;
using System.Web;

namespace API_Project.Controllers.Registration
{
    [RoutePrefix("api/reg")]
    public class RegController : ApiController
    {
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
            if (string.IsNullOrEmpty(user.UserName)) return BadRequest("Incorrect user data");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            DataValidator.DataValidator validator = new DataValidator.DataValidator();
            if (!validator.ValidateEmail(user.Login)) return BadRequest("Incorrect user data");
            if (!validator.ValidateMD5(user.Password)) return BadRequest("Incorrect user data");

            if (_db.Users.Find(user.Login) != null) return BadRequest("This email already registration");

            if (_db.Roles.Where(prop => prop.Title == user.Role).FirstOrDefault() == null) return BadRequest("Incorrect user data");
            #endregion

            DataBaseCore.User newUser = new DataBaseCore.User()
            {
                UserName = user.UserName,
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
                if (!SendVerificationCode(newUser, emailState.VerificationCode)) return BadRequest("The limit of sent messages has been exceeded");
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
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
                bool isSendSuccess = SendVerificationSuccess(user);
                if (!isSendSuccess) return BadRequest("The limit of sent messages has been exceeded");
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok(new UserModel(user));
        }
        #endregion

        #region Reset password
        /// <summary>
        /// Allows you to reset the password, sends the code to the account email
        /// </summary>
        /// <param name="login">Account email</param>
        /// <returns>Returns http action result code</returns>
        [HttpPost]
        [Route("resetPassword")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostResetPassword([FromBody] string login)
        {
            #region Validating
            if (string.IsNullOrEmpty(login)) return BadRequest("Incorrect email");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(login);
            if (user == null) return NotFound();
            #endregion

            Random rnd = new Random();
            user.EmailState.VerificationCode = rnd.Next(23981, 100000);
            user.EmailState.DateSentCode = DateTime.Now;

            try
            {
                _db.SaveChanges();
                bool isSendSuccess = SendResetPasswordCode(user, user.EmailState.VerificationCode);
                if (!isSendSuccess) return BadRequest("The limit of sent messages has been exceeded");
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok();
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
            #endregion

            if (user.EmailState.DateSentCode.AddSeconds(600) < DateTime.Now) return BadRequest("Verificated code expired");
            if (user.EmailState.VerificationCode != resetPasswordData.Code) return BadRequest("Incorrect verification code");

            user.EmailState.VerificationCode = -1;

            user.Password = resetPasswordData.Password;
            try
            {
                _db.SaveChanges();
                bool isSendSuccess = SendResetPasswordSuccess(user);
                if (!isSendSuccess) return BadRequest("The limit of sent messages has been exceeded");
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok(new UserModel(user));
        }
        #endregion

        [HttpGet]
        [Route("")]
        public IHttpActionResult Getlol()
        {
            

            return Ok();
        }

        private bool SendVerificationCode(DataBaseCore.User user, int code)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.ConfirmEmailMessage(user.Login, code));
        }

        private bool SendVerificationSuccess(DataBaseCore.User user)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            return sender.SimpleSend(new EmailSender.Messages.SuccessConfirmEmailMessage(user.Login, user.UserName));
        }

        private bool SendResetPasswordCode(DataBaseCore.User user, int code)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            string subject = "Go Study - восстановление пароля";
            string messege = "Ваш код для восстановления пароля учётной записи: " + code + "\nВведите код в приложение, для сброса пароля.\nНе сообщайте никому цифры кода!";
            return sender.SimpleSend(user.Login, subject, messege, user.UserName);
        }

        private bool SendResetPasswordSuccess(DataBaseCore.User user)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            string subject = "Go Study - пароль изменён";
            string messege = "Вы успешно сбросили пароль вашей учётной записи. Если это сделали не Вы, и считаете это подозрительным, ответьте на это сообщение.";
            return sender.SimpleSend(user.Login, subject, messege, user.UserName);
        }
        protected override void Dispose(bool disposing) => base.Dispose(disposing);

    }
}