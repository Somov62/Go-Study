using System;
using System.IO;
using System.Net.Mail;
using System.Web;

namespace EmailSender.Messages
{
    public class LoginMessage : MailMessage
    {
        public LoginMessage(string email, DateTime date, string ipAddress) :
            base(new MailAddress("gostudymailsender@gmail.com", "BOT"),
                   new MailAddress(email, ""))
        {
            base.Subject = "Выполнен вход в аккаунт Go Study!";
            string pathToHtml = Path.Combine(HttpRuntime.AppDomainAppPath.Replace("\\API_Project", ""), "EmailSender", "Messages", "Views", "LoginMessage.html");
            base.Body = File.ReadAllText(pathToHtml).Replace("REPLACE_IPADRESS", ipAddress).Replace("REPLACE_DATE", date.ToString("F"));
            base.IsBodyHtml = true;
        }

    }
}
