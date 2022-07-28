using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Timers;

namespace EmailSender
{
    public class EmailSender
    {
        private static int _countSentEmails = 0;
        private static readonly Timer _timer = new Timer()
        {
            AutoReset = true,
            Enabled = true,
            Interval = 86400
        };
        public EmailSender()
        {
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Elapsed += Timer_Elapsed;
            PathToAccessFile = Path.Combine(System.Web.HttpRuntime.AppDomainAppPath.Replace("API_Project", ""), "Credentilas", "EmailPassword.txt");
        }

        public string PathToAccessFile { get; }
        public static int ConutSentEmails => _countSentEmails;

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _countSentEmails = 0;
        }

        private async Task SendEmailAsync(MailMessage message)
        {
            
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("gostudymailsender@gmail.com", File.ReadAllText(PathToAccessFile)),
                EnableSsl = true
            };
            await smtp.SendMailAsync(message);
            _countSentEmails++;
        }

        public bool SimpleSend(string email, string subject, string messege, string recipientName = "Someone")
        {
            if (_countSentEmails == 200)
            {
                return false;
                //throw new System.Exception("The limit of sent messages has been exceeded");
            }
            MailAddress from = new MailAddress("gostudymailsender@gmail.com", "BOT");
            MailAddress to = new MailAddress(email, recipientName);
            MailMessage mail = new MailMessage(from, to)
            {
                Subject = subject,
                Body = messege
            };
            Task.Factory.StartNew(() => SendEmailAsync(mail));
            return true;
        }
        public bool SimpleSend(MailMessage message)
        {
            if (_countSentEmails == 200)
            {
                return false;
                //throw new System.Exception("The limit of sent messages has been exceeded");
            }
            Task.Factory.StartNew(() => SendEmailAsync(message));
            return true;
        }
    }
}
