using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Blogging.UnitTests
{
    public static class Helpers
    {
        public static void EnsureOkObjectResult(this IActionResult actionResult)
        {
            actionResult.Should().BeOfType(typeof(OkObjectResult));
            actionResult.As<OkObjectResult>().Value.Should().NotBeNull();
        }

        public static void EnsureOkResult(this IActionResult actionResult)
        {
            actionResult.Should().BeOfType(typeof(OkResult));
        }

        public static T GetObjectResult<T>(this IActionResult actionResult)
        {
            var objectResult = (OkObjectResult) actionResult;
            var value = objectResult.Value.As<T>();
            value.Should().NotBeNull();
            return value;
        }

        public static void EnsureStatusCode(this IActionResult actionResult, int statusCode)
        {
            var objectResult = (IStatusCodeActionResult) actionResult;
            objectResult.StatusCode.Should().Be(statusCode);
        }
    }
}