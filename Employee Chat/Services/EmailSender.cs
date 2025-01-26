using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Employee Management And Chat App", _configuration["Smtp:Username"])); // Match SMTP username
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = message };

        using (var client = new SmtpClient())
        {
            // Retrieve SMTP configuration
            var smtpServer = _configuration["Smtp:Server"];
            var smtpPortString = _configuration["Smtp:Port"];
            var smtpUser = _configuration["Smtp:Username"];
            var smtpPass = _configuration["Smtp:Password"];

            // Validate configuration values
            if (string.IsNullOrWhiteSpace(smtpServer) || string.IsNullOrWhiteSpace(smtpUser) || string.IsNullOrWhiteSpace(smtpPass))
            {
                throw new InvalidOperationException("SMTP configuration is missing in appsettings.json.");
            }

            if (!int.TryParse(smtpPortString, out var smtpPort))
            {
                throw new ArgumentException("Invalid SMTP port configuration.");
            }

            try
            {
                // Connect to Gmail's SMTP server
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.SslOnConnect);

                // Authenticate with Gmail
                await client.AuthenticateAsync(smtpUser, smtpPass);

                // Send the email
                await client.SendAsync(emailMessage);

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                throw;
            }
            finally
            {
                // Disconnect
                await client.DisconnectAsync(true);
            }
        }
    }
}
