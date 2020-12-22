using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Controllers.TwoFactor
{
    public class TwoFactorViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool CodeRequestCooldown { get; set; }
        public bool HasNewCodeBeenReSent { get; set; }
        public DateTime AllowedToResendAt { get; set; }

        public string Digit1 { get; set; }
        public string Digit2 { get; set; }
        public string Digit3 { get; set; }
        public string Digit4 { get; set; }
        public string Digit5 { get; set; }
        public string Digit6 { get; set; }


        [StringLength(6, MinimumLength = 6, ErrorMessage = "The code you entered is invalid")]
        public string InputCode => Digit1 + Digit2 + Digit3 + Digit4 + Digit5 + Digit6;
    }
}