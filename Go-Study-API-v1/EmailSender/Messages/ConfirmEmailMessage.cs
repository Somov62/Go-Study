using System.IO;
using System.Net.Mail;
using System.Web;

namespace EmailSender.Messages
{
    public class ConfirmEmailMessage : MailMessage
    {
        public ConfirmEmailMessage(string email, int code) :
            base ( new MailAddress("gostudymailsender@gmail.com", "BOT"),
                   new MailAddress(email, ""))
        {
            base.Subject = "Go Study - завершение регистрации";
            string pathToHtml = Path.Combine(HttpRuntime.AppDomainAppPath.Replace("\\API_Project", ""), "EmailSender", "Messages", "Views", "ConfirmEmailMessage.html");
            base.Body = File.ReadAllText(pathToHtml).Replace("REPLACE_CODE", code.ToString());
            base.IsBodyHtml = true;
        }
    }
}
