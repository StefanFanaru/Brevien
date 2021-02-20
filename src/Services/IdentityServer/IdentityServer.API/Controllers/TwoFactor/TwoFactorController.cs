using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.API.Common.Constants;
using IdentityServer.API.Data;
using IdentityServer.API.Data.Entities;
using IdentityServer.API.Services.Verification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityServer.API.Controllers.TwoFactor
{
    [Authorize]
    public class TwoFactorController : Controller
    {
        private readonly IdentityContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITwoFactorVerification _verification;

        public TwoFactorController(UserManager<ApplicationUser> userManager, ITwoFactorVerification verification,
            IdentityContext context)
        {
            _userManager = userManager;
            _verification = verification;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(bool codeResent = false)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.PhoneNumberConfirmed)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var model = new TwoFactorViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CodeRequestCooldown = codeResent, // signals if we are in the context of a "Send code again" request
                AllowedToResendAt = DateTime.UtcNow.AddSeconds(AppConstants.TwoFactorCooldownSeconds) // Needed for the JS timer
            };

            var hasEmittedCodes = await _context.TwoFactorStatuses
                .Where(e => e.UserId == user.Id).AnyAsync();

            if (hasEmittedCodes)
            {
                // TODO: check why this is ordered like this
                var lastTwoFactorCodeDate = await _context.TwoFactorStatuses
                    .Where(e => e.UserId == user.Id)
                    .Select(e => e.DateSent)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefaultAsync();

                // If the a code has already been send and that was less than X seconds ago, live timer will be shown with the current value (X = cooldown time)
                if (lastTwoFactorCodeDate.AddSeconds(AppConstants.TwoFactorCooldownSeconds) > DateTime.UtcNow)
                {
                    // The JS timer will start with the actual time needed to wait
                    model.AllowedToResendAt = lastTwoFactorCodeDate.AddSeconds(AppConstants.TwoFactorCooldownSeconds);
                    return View(model);
                }

                // Will print some helping text on the view relative to the current case
                if (codeResent)
                {
                    model.HasNewCodeBeenReSent = true;
                }
            }

            // 2FA Code is sent via SMS
            var result = await _verification.StartVerificationAsync(user.PhoneNumber, user.Id);

            if (result.Errors?.Count > 0)
                foreach (var error in result.Errors)
                    Log.Error(error);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPhoneNumber(TwoFactorViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var result = await _verification.CheckVerificationAsync(user.PhoneNumber, model.InputCode);
                if (result.IsValid)
                {
                    user.PhoneNumberConfirmed = true;
                    await _userManager.UpdateAsync(user);

                    _context.TwoFactorStatuses
                        .Where(e => e.UserId == user.Id)
                        .ToList()
                        .ForEach(t => _context.Remove(t));

                    await _context.SaveChangesAsync();

                    Log.Information($"Phone number {user.PhoneNumber} verified for user {user.Email}");

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    Log.Error($"Verification Failed: {error}");

                    ModelState.AddModelError("InvalidCode", error);
                }
            }

            // If there are validation errors, we reset the state regarding "code resent"
            model.HasNewCodeBeenReSent = false;
            model.CodeRequestCooldown = false;
            return View(model);
        }
    }
}