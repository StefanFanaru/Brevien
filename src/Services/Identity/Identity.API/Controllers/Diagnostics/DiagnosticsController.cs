using System.Threading.Tasks;
using Identity.API.Common;
using Identity.API.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Identity.API.Controllers.Diagnostics
{
    [SecurityHeaders]
    [Authorize]
    public class DiagnosticsController : Controller
    {
        private readonly IConfiguration _configuration;

        public DiagnosticsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            if (_configuration.GetSection("Settings:Environment").Value != Environments.Local) return NotFound();

            var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
            return View(model);
        }
    }
}