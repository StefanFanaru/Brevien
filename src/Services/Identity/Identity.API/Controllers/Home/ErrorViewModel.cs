﻿using IdentityServer4.Models;

namespace Identity.API.Controllers.Home
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage {Error = error};
        }

        public ErrorMessage Error { get; set; }
    }
}