using System.Collections.Generic;
using Posting.Core.Errors;

namespace Posting.API.Helpers
{
    public class StatusCodeMapping
    {
        public static IDictionary<string, int> ErrorsToStatusCodes = new Dictionary<string, int>
        {
            {ErrorCodes.BadArgument, 400},
            {ErrorCodes.EntityNotFound, 404},
            {ErrorCodes.SystemError, 500}
        };

        public static int ResolveStatusCode(string errorCode)
        {
            if (ErrorsToStatusCodes.ContainsKey(errorCode))
            {
                return ErrorsToStatusCodes[errorCode];
            }

            return 500;
        }
    }
}