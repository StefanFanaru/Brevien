using Microsoft.AspNetCore.Mvc;
using Posting.Core.Interfaces.Asp;

namespace Posting.API.Helpers
{
    public static class ControllerExtensions
    {
        public static IActionResult Result<T>(this Controller controller, IOperationResult<T> result)
        {
            return new OperationActionResult<T>(result);
        }
    }
}