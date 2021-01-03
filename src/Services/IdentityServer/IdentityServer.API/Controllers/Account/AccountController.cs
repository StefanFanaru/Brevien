using System;
using System.Threading.Tasks;
using System.Web;
using Ardalis.GuardClauses;
using IdentityServer.API.Common;
using IdentityServer.API.Data.Entites;
using IdentityServer.API.Services.Interfaces;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer.API.Controllers.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ApplicationUrls _applicationUrls;
        private readonly IEmailSender _emailSender;
        private readonly IEventService _events;
        private readonly GeneralConfiguration _generalConfiguration;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IEventService events,
            ILogger<AccountController> logger,
            IOptions<GeneralConfiguration> configuration,
            IOptions<ApplicationUrls> applicationUrls,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
            _logger = logger;
            _emailSender = emailSender;
            _generalConfiguration = configuration.Value;
            _applicationUrls = applicationUrls.Value;
        }

        /// <summary>
        ///     Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var vm = new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                ReturnUrl = returnUrl
            };

            return View(vm);
        }

        /// <summary>
        ///     Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName,
                        clientId: context?.Client.ClientId));

                    if (context != null)
                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);

                    if (string.IsNullOrEmpty(model.ReturnUrl)) return Redirect(_applicationUrls.AngularClient);

                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials",
                    clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            var vm = new LoginViewModel
            {
                RememberLogin = model.RememberLogin,
                Username = model.Username,
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                ReturnUrl = model.ReturnUrl
            };

            return View(vm);
        }


        /// <summary>
        ///     Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = new LogoutViewModel {LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt};
            var context = await _interaction.GetLogoutContextAsync(logoutId);

            if (User?.Identity.IsAuthenticated != true)
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;

            if (context?.ShowSignoutPrompt == false)
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;

            if (vm.ShowLogoutPrompt == false)
                // if the request for logout was properly authenticated from IdentityServer.API, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);

            return View(vm);
        }

        /// <summary>
        ///     Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            return Redirect(_applicationUrls.AngularClient);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult InvalidLink()
        {
            return View();
        }


        /// <summary>
        ///     Show register page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        /// <summary>
        ///     Handle register page postback
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Register(RegisterInputModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser =
                    ApplicationUser.Create(model.Email, model.LastName, model.FirstName, model.AcceptsInformativeEmails);

                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"AccountCreated with Id: {newUser.Id}");

                    var registerConfirmationModel = new RegisterConfirmationViewModel
                    {
                        Email = newUser.Email,
                        FullName = $"{newUser.FirstName} {newUser.LastName}"
                    };
                    // TODO: Send confirmation email and setup needed pagesc

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    var encodedToken = HttpUtility.UrlEncode(token);
                    var link =
                        $"{_applicationUrls.IdentityServer}/Account/emailConfirmation?email={newUser.Email}&token={encodedToken}";

                    await _emailSender.SendEmailAsync(newUser.Email, newUser.GetFullName(), "Pleas confirm your email account.",
                        link,
                        "Hello, this is some plain text from Identity.");

                    return RedirectToAction("RegisterConfirmation", registerConfirmationModel);
                }

                foreach (var error in result.Errors) ModelState.AddModelError("RegisterErrors", error.Description);
            }

            return View();
        }

        [HttpGet]
        public IActionResult RegisterConfirmation(RegisterConfirmationViewModel viewModel)
        {
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EmailConfirmation(string email, string token)
        {
            Guard.Against.NullOrEmpty(email, nameof(token));
            Guard.Against.NullOrEmpty(token, nameof(token));

            var user = await _userManager.FindByEmailAsync(email);

            Guard.Against.Null(user, nameof(user));

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                var model = new EmailConfirmationViewModel
                {
                    Email = user.Email,
                    FullName = user.GetFullName()
                };
                return View(model);
            }

            return RedirectToAction("InvalidLink");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var user = await _userManager.FindByEmailAsync(viewModel.Email);

            Guard.Against.Null(user, nameof(user));

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var link = $"{_applicationUrls.IdentityServer}/Account/resetPassword?email={viewModel.Email}&token={encodedToken}";

            await _emailSender.SendEmailAsync(viewModel.Email, user.GetFullName(),
                "Hello, you requested a new password for your account,you can reset it by click the link."
                , link, "Hello, this is some plain text from Identity.");
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel {Token = token, Email = email};
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid) return View(resetPasswordModel);

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            Guard.Against.Null(user, nameof(user));

            var resetPassResult =
                await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors) ModelState.TryAddModelError(error.Code, error.Description);

                return View();
            }

            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}