using System.IO;
using System.Net.Mail;
using System.Web;

namespace EmailSender.Messages
{
    public class ConfirmResetPasswordMessage : MailMessage
    {
        public ConfirmResetPasswordMessage(string email, int code, string cancelRequestPath) :
            base(new MailAddress("gostudymailsender@gmail.com", "BOT"),
                   new MailAddress(email, ""))
        {
            base.Subject = "Go Study - восстановление пароля";
            string pathToHtml = Path.Combine(HttpRuntime.AppDomainAppPath.Replace("\\API_Project", ""), "EmailSender", "Messages", "Views", "ConfirmResetPasswordMessage.html");
            base.Body = File.ReadAllText(pathToHtml).Replace("REPLACE_CODE", code.ToString()).Replace("REPLACE_ADRESS", cancelRequestPath);
            base.IsBodyHtml = true;
        }
    }
}
