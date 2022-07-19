using System.IO;
using System.Net.Mail;
using System.Web;

namespace EmailSender.Messages
{
    public class SuccessResetPasswordMessage : MailMessage
    {
        public SuccessResetPasswordMessage(string email, string username,  string freezeRequestPath) :
            base(new MailAddress("gostudymailsender@gmail.com", "BOT"),
                   new MailAddress(email, username))
        {
            base.Subject = "Go Study - сброс пароля прошёл успешно";
            string pathToHtml = Path.Combine(HttpRuntime.AppDomainAppPath.Replace("\\API_Project", ""), "EmailSender", "Messages", "Views", "SuccessResetPasswordMessage.html");
            base.Body = File.ReadAllText(pathToHtml).Replace("USERNAME", username).Replace("REPLACE_ADRESS", freezeRequestPath);
            base.IsBodyHtml = true;
        }
    }
}
