using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApplication1.Interface;

namespace WebApplication1.Service
{
    public class MailService : IEmail
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        public MailService(IConfiguration configuration)
        {
            var smtpsetting = configuration.GetSection("SMTP");
            _smtpHost = smtpsetting.GetValue<string>("SMTP_HOST") ?? throw new InvalidOperationException("Host is not configured");
            _smtpPort = smtpsetting.GetValue<int>("SMTP_PORT");
            _smtpUser = smtpsetting.GetValue<string>("SMTP_USER") ?? throw new InvalidOperationException("User is not configured");
            _smtpPassword = smtpsetting.GetValue<string>("SMTP_PASSWORD") ?? throw new InvalidOperationException("Password is not connected");
        }
        public async Task SendEmailAsync(string to, string subject, string body, bool ishtml = false)
        {

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true,

            };
            var mailmessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = ishtml,
            };
            mailmessage.To.Add(to);

            try
            {
                await client.SendMailAsync(mailmessage);
                Console.WriteLine("Mail sent suceesfully");
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("Failed to send mail", ex);
            }

        }
    }
}