using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Identity.API.Common;
using Identity.Data;
using Identity.Data.Entities;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace Identity.API.Services.Verification
{
    public class TwoFactorTwoFactorVerification : ITwoFactorVerification
    {
        private readonly TwilioConfiguration _configuration;
        private readonly IdentityContext _context;

        public TwoFactorTwoFactorVerification(IOptions<TwilioConfiguration> configuration, IdentityContext context)
        {
            _context = context;
            _configuration = configuration.Value;
            TwilioClient.Init(_configuration.AccountSid, _configuration.AuthToken);
        }

        public async Task<VerificationResult> StartVerificationAsync(string phoneNumber, string userId, string channel = "sms")
        {
            VerificationResult result;
            try
            {
                var verificationResource = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: channel,
                    pathServiceSid: _configuration.VerificationSid
                );
                result = new VerificationResult(verificationResource.Sid);
            }
            catch (TwilioException e)
            {
                return new VerificationResult(new List<string> {e.Message});
            }

            if (result.IsValid)
            {
                var twoFactorStatus = new TwoFactorStatus(userId);

                await _context.TwoFactorStatuses.AddAsync(twoFactorStatus);
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code)
        {
            try
            {
                var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                    to: phoneNumber,
                    code: code,
                    pathServiceSid: _configuration.VerificationSid
                );
                return verificationCheckResource.Status.Equals("approved")
                    ? new VerificationResult(verificationCheckResource.Sid)
                    : new VerificationResult(new List<string> {"Wrong code. Try again."});
            }
            catch (TwilioException e)
            {
                return new VerificationResult(new List<string> {e.Message});
            }
        }
    }
}
