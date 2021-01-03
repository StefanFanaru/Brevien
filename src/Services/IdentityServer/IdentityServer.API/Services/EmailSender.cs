using System.Threading.Tasks;
using IdentityServer.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IdentityServer.API.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string recipientEmail, string recipientName, string subject, string htmlContent,
            string plainTextContent = "")
        {
            var apiKey = _configuration["SecretKeys:SendGridApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("office@brevien.com", "Brevien");
            var to = new EmailAddress(recipientEmail, recipientName);

            var email = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            await client.SendEmailAsync(email);
        }
    }
}