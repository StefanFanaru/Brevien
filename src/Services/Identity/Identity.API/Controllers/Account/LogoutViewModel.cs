﻿namespace Identity.API.Controllers.Account
{
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }
}