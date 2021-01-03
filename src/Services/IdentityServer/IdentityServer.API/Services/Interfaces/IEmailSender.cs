using System.Threading.Tasks;

namespace IdentityServer.API.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string recipientEmail, string recipientName, string subject, string htmlContent,
            string plainTextContent = "");
    }
}