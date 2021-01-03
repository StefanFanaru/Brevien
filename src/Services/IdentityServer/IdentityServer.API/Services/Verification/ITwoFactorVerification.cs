using System.Threading.Tasks;

namespace IdentityServer.API.Services.Verification
{
    public interface ITwoFactorVerification
    {
        Task<VerificationResult> StartVerificationAsync(string phoneNumber, string userId, string channel = "sms");

        Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code);
    }
}