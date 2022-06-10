using API_Project.Models;
using DataBaseCore;
using System;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Project.Controllers.Registration
{
    public class RegController : ApiController
    {
        private readonly DbEntities _db = DbEntities.GetContext();

        #region Registration new user
        [Route("api/Reg")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostUser(UserModel user)
        {
            if (string.IsNullOrEmpty(user.Login)) return BadRequest("Incorrect user data");
            if (string.IsNullOrEmpty(user.Password)) return BadRequest("Incorrect user data");
            if (string.IsNullOrEmpty(user.UserName)) return BadRequest("Incorrect user data");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            DataValidator.DataValidator validator = new DataValidator.DataValidator();
            if (!validator.ValidateEmail(user.Login)) return BadRequest("Incorrect user data");
            if (!validator.ValidateMD5(user.Password)) return BadRequest("Incorrect user data");

            if (_db.Users.Find(user.Login) != null) return BadRequest("This email already registration");

            if (_db.Roles.Find(user.RoleId) == null) return BadRequest("Incorrect user data");
            user.AccessTypeId = 1;

            DataBaseCore.User newUser = new DataBaseCore.User()
            {
                UserName = user.UserName,
                Login = user.Login,
                Password = user.Password,
                RoleId = user.RoleId,
                AccessTypeId = 1
            };

            Random rnd = new Random();

            DataBaseCore.EmailState emailState = new DataBaseCore.EmailState()
            {
                Login = user.Login,
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

        [Route("api/Reg/CheckVerificationCode")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostCheckVerificationCode(UserModel userModel, int code)
        {
            if (string.IsNullOrEmpty(userModel.Login)) return BadRequest("Incorrect email");
            if (code < 23981 || code > 100000) return BadRequest("Incorrect verification code");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(userModel.Login);
            if (user == null) return NotFound();
            if (user.EmailState.IsVerificated) return BadRequest("Email already verificated");

            if (user.EmailState.DateSentCode.AddSeconds(600) < DateTime.Now) return BadRequest("Verificated code expired");
            if (user.EmailState.VerificationCode != code) return BadRequest("Incorrect verification code");

            user.EmailState.IsVerificated = true;
            //user.EmailState.VerificationCode = -1;


            try
            {
                _db.SaveChanges();
                if (SendVerificationSuccess(user)) return BadRequest("The limit of sent messages has been exceeded");
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
        [Route("api/Reg/ResetPassword")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostResetPassword(string login)
        {
            if (string.IsNullOrEmpty(login)) return BadRequest("Incorrect email");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(login);
            if (user == null) return NotFound();

            Random rnd = new Random();
            user.EmailState.VerificationCode = rnd.Next(23981, 100000);
            user.EmailState.DateSentCode = DateTime.Now;


            try
            {
                _db.SaveChanges();
                if (!SendResetPasswordCode(user, user.EmailState.VerificationCode)) return BadRequest("The limit of sent messages has been exceeded");
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok();
        }

        [Route("api/Reg/CheckResetPasswordCode")]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult PostCheckResetPasswordCode(string login, string newPassword, int code)
        {
            if (string.IsNullOrEmpty(login)) return BadRequest("Incorrect email");
            if (string.IsNullOrEmpty(newPassword)) return BadRequest("Incorrect password");

            DataValidator.DataValidator validator = new DataValidator.DataValidator();
            if (!validator.ValidateMD5(newPassword)) return BadRequest("Incorrect password");

            if (code < 23981 || code > 100000) return BadRequest("Incorrect verification code");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _db.Users.Find(login);
            if (user == null) return NotFound();

            if (user.EmailState.DateSentCode.AddSeconds(600) < DateTime.Now) return BadRequest("Verificated code expired");
            if (user.EmailState.VerificationCode != code) return BadRequest("Incorrect verification code");

            //user.EmailState.VerificationCode = -1;

            user.Password = newPassword;
            try
            {
                _db.SaveChanges();
                if (!SendResetPasswordSuccess(user)) return BadRequest("The limit of sent messages has been exceeded");
            }
            catch
            {
                //Logger
                return BadRequest("Something went wrong");
            }
            return Ok(new UserModel(user));
        }

        #endregion

        private bool SendVerificationCode(User user, int code)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            string subject = "Go Study - завершение регистрации";
            string messege = "Ваш код для подтверждения электронной почты: " + code + "\nВведите код в приложение, для завершения регистрации.\nНе сообщайте никому цифры кода!";
            return sender.SimpleSend(user.Login, subject, messege, user.UserName);
        }

        private bool SendVerificationSuccess(User user)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            string subject = "Добро пожаловать в Go Study!";
            string messege = "Вы успешно завершили регистрацию и теперь Вам открыт доступ к приложению. Надеемся, что Go Study будет для Вас отличным помощником в поиске вашего будущего учебного учреждения и чутким наставником на момент обучения! Желаем Вам удачи!\n\nВсегда Ваша - команда Go Study";
            return sender.SimpleSend(user.Login, subject, messege, user.UserName);
        }

        private bool SendResetPasswordCode(User user, int code)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            string subject = "Go Study - восстановление пароля";
            string messege = "Ваш код для восстановления пароля учётной записи: " + code + "\nВведите код в приложение, для сброса пароля.\nНе сообщайте никому цифры кода!";
            return sender.SimpleSend(user.Login, subject, messege, user.UserName);
        }

        private bool SendResetPasswordSuccess(User user)
        {
            EmailSender.EmailSender sender = new EmailSender.EmailSender();
            string subject = "Go Study - пароль изменён";
            string messege = "Вы успешно сбросили пароль вашей учётной записи. Если это сделали не Вы, и считаете это подозрительным, ответьте на это сообщение.";
            return sender.SimpleSend(user.Login, subject, messege, user.UserName);
        }
        protected override void Dispose(bool disposing) => base.Dispose(disposing);

    }
}