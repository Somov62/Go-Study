using System.IO;
using System.Net.Mail;
using System.Web;

namespace EmailSender.Messages
{
    public class FreezeAccountMessage : MailMessage
    {
        public FreezeAccountMessage(string email, string username) :
            base(new MailAddress("gostudymailsender@gmail.com", "BOT"),
                   new MailAddress(email, username))
        {
            base.Subject = "Go Study - ваш аккаунт заморожен!";
            string pathToHtml = Path.Combine(HttpRuntime.AppDomainAppPath.Replace("\\API_Project", ""), "EmailSender", "Messages", "Views", "FreezeAccountMessage.html");
            base.Body = File.ReadAllText(pathToHtml).Replace("USERNAME", username);
            base.IsBodyHtml = true;
        }
    }
}
