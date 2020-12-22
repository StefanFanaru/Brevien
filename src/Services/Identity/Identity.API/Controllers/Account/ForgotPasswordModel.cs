using System.ComponentModel.DataAnnotations;

namespace Identity.API.Controllers.Account
{
    public class ForgotPasswordModel
    {
        [Required] [EmailAddress] public string Email { get; set; }
    }
}