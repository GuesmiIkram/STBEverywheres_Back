using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace STBEverywhere_back_APIClient.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("STB_Everywhere", emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public async Task SendEmailWithAttachmentsAsync(string email, string subject, string body, List<string> filePaths)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("STB_Everywhere", emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.TextBody = body;

            // Ajouter les pièces jointes
            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    builder.Attachments.Add(filePath);
                }
            }

            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }


    }
}