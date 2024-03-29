﻿using BinhDinhFoodWeb.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace BinhDinhFoodWeb.Intefaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _configuration;
        public MailService(IOptions<MailSettings> mailSettings, IConfiguration configuration)
        {
            _mailSettings = mailSettings.Value;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            string mail = _configuration["MailSettings:Mail"];//_mailSettings.Mail;
           // if (string.IsNullOrEmpty(mail)) mail = "twoonez@outlook.com";
            email.Sender = MailboxAddress.Parse(mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            string host = _configuration["MailSettings:Host"];//"smtp -mail.outlook.com";
            string password = _configuration["MailSettings:Password"]; //"Iknowwhoyouare1001";
            smtp.Connect(host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(mail, password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
