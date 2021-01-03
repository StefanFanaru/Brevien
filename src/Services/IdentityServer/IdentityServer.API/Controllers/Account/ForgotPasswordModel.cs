using System.ComponentModel.DataAnnotations;

namespace IdentityServer.API.Controllers.Account
{
    public class ForgotPasswordModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}