using System;
using System.Net;
using MediatR;
using Posting.Core.Errors;
using Posting.Core.Interfaces.Asp;

namespace Posting.Infrastructure.Operations
{
    public class ErrorResultBuilder<T>
    {
        private readonly ErrorBuilder _errorBuilder;

        public ErrorResultBuilder(HttpStatusCode statusCode, string message)
        {
            _errorBuilder = new ErrorBuilder(statusCode, message);
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
        public static ErrorResultBuilder<T> Error<T>(HttpStatusCode statusCode, string message)
        {
            return new(statusCode, message);
        }

        public static IOperationResult<T> Ok<T>(T result)
        {
            return new OperationResult<T>(result, HttpStatusCode.OK);
        }

        public static IOperationResult<Unit> Ok()
        {
            return new OperationResult<Unit>(HttpStatusCode.OK);
        }

        public static IOperationResult<T> Created<T>(T result)
        {
            return new OperationResult<T>(result, HttpStatusCode.Created);
        }
    }
}