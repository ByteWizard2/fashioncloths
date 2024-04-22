using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace E_commercePro.services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string email, string emailSubject, string emailMessage)
        {
            var smtpClient = new SmtpClient
            {
                Host = _smtpSettings.SmtpServer,
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            };

 

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = emailSubject,
                Body = emailMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<string> GenerateOTPAsync(int length = 6)
        {
            var random = new Random();
            var otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 9).ToString();
            }
            return otp;
        }
    }
}
