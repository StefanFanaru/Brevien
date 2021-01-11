using Posting.Core.Errors;
using Posting.Core.Interfaces.Asp;

namespace Posting.Infrastructure.Operations
{
    public class OperationResult<TResult> : IOperationResult<TResult>
    {
        public OperationResult(bool isSuccess, bool hasResult, Error error, TResult result)
        {
            Result = result;
            IsSuccess = isSuccess;
            Error = error;
            HasResult = hasResult;
        }

        public OperationResult() : this(true, false, null, default)
        {
        }

        public OperationResult(TResult result) : this(true, true, null, result)
        {
        }

        public OperationResult(Error error) : this(false, false, error, default)
        {
        }

        public bool HasResult { get; }
        public TResult Result { get; }
        public bool IsSuccess { get; }
        public Error Error { get; }
    }
}