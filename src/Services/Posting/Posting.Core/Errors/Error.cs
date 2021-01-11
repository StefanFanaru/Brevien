using System.Collections.Generic;

namespace Posting.Core.Errors
{
    public class Error
    {
        public Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public string Code { get; }
        public string Message { get; }
        public string Target { get; set; }
        public List<Error> Details { get; set; }
    }
}