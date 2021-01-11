using System;
using MediatR;
using Posting.Core.Errors;
using Posting.Core.Interfaces.Asp;

namespace Posting.Infrastructure.Operations
{
    public class ErrorResultBuilder<T>
    {
        private readonly ErrorBuilder _errorBuilder;

        public ErrorResultBuilder(string responseErrorCode, string responseMessage)
        {
            _errorBuilder = new ErrorBuilder(responseErrorCode, responseMessage);
        }

        public IOperationResult<T> Build()
        {
            return new OperationResult<T>(_errorBuilder.Build());
        }

        public ErrorResultBuilder<T> ForTarget(string targetString)
        {
            _errorBuilder.ForTarget(targetString);
            return this;
        }

        public ErrorResultBuilder<T> WithDetailsError(Func<ErrorBuilder> detailsErrorBuilder)
        {
            _errorBuilder.WithErrorDetails(detailsErrorBuilder);
            return this;
        }

        public ErrorResultBuilder<T> WithInner(Func<InnerErrorBuilder> builderAction)
        {
            _errorBuilder.WithInner(builderAction);
            return this;
        }
    }

    public class ResultBuilder
    {
        public static ErrorResultBuilder<T> Error<T>(string responseErrorCode, string responseMessage)
        {
            return new(responseErrorCode, responseMessage);
        }

        public static IOperationResult<T> Ok<T>(T result)
        {
            return new OperationResult<T>(result);
        }

        public static IOperationResult<Unit> Ok()
        {
            return new OperationResult<Unit>();
        }
    }
}