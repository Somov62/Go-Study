using System.IO;
using System.Net.Mail;
using System.Web;

namespace EmailSender.Messages
{
    public class SuccessResetPasswordMessage : MailMessage
    {
        public SuccessResetPasswordMessage(string email, string username) :
            base(new MailAddress("gostudymailsender@gmail.com", "BOT"),
                   new MailAddress(email, username))
        {
            base.Subject = "Добро пожаловать в Go Study!";
            string pathToHtml = Path.Combine(HttpRuntime.AppDomainAppPath.Replace("\\API_Project", ""), "EmailSender", "Messages", "Views", "SuccessResetPasswordMessage.html");
            base.Body = File.ReadAllText(pathToHtml).Replace("USERNAME", username);
            base.IsBodyHtml = true;
        }
    }
}
