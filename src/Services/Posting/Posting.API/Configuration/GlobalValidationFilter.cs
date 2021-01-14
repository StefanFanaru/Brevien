using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Posting.Core.Errors;
using Posting.Infrastructure.Operations;

namespace Posting.API.Configuration
{
    public class GlobalValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
            {
                var builder = ResultBuilder.Error<object>(HttpStatusCode.BadRequest, "Invalid parameters provided in request")
                    .ForTarget("request");

                foreach (var error in modelState.Keys.SelectMany(key =>
                    modelState[key].Errors.Select(x => new
                    {
                        Key = key,
                        ErrorMessage = string.IsNullOrEmpty(x.ErrorMessage) ? x.Exception?.Message : x.ErrorMessage
                    })))
                {
                    builder.WithDetailsError(() =>
                        new ErrorBuilder(HttpStatusCode.BadRequest, error.ErrorMessage).ForTarget(error.Key));
                }

                context.Result = new ObjectResult(builder.Build());
            }
        }
    }
}