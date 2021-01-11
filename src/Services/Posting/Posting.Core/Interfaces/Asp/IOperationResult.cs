using Posting.Core.Errors;

namespace Posting.Core.Interfaces.Asp
{
    public interface IOperationResult<out TResult>
    {
        TResult Result { get; }
        bool IsSuccess { get; }
        Error Error { get; }
        public bool HasResult { get; }
    }
}