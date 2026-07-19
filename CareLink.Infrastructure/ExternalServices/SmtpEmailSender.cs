using CareLink.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CareLink.Infrastructure.ExternalServices
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendAsync(string toEmail, string subject, string body)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var host = smtpSection["Host"];

            if (string.IsNullOrWhiteSpace(host))
            {
                _logger.LogWarning(
                    "SMTP is not configured. Email was not sent. Recipient: {ToEmail}, Subject: {Subject}",
                    toEmail, subject);
                return false;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(smtpSection["FromAddress"]));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;
                message.Body = new TextPart("plain") { Text = body };

                using var client = new SmtpClient();

                var port = int.Parse(smtpSection["Port"] ?? "587");

                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpSection["Username"], smtpSection["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
                return false;
            }
        }
    }
}