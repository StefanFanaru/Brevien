using System.Threading.Tasks;
using Identity.API.Common;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.API.Controllers.Home
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationUrls _applicationUrls;
        private readonly IWebHostEnvironment _environment;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger _logger;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment,
            ILogger<HomeController> logger,
            IOptions<ApplicationUrls> applicationUrls)
        {
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
            _applicationUrls = applicationUrls.Value;
        }

        public IActionResult Index()
        {
            if (User?.Identity.IsAuthenticated == true)
            {
                if (_environment.IsDevelopment()) return RedirectToAction("Index", "Diagnostics");

                return Redirect(_applicationUrls.AngularClient); // TODO: Redirect to profile page
            }

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        ///     Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                    // only show in development
                    message.ErrorDescription = null;
            }

            return View("Error", vm);
        }
    }
}