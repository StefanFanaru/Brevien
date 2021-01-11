using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Posting.Core.Errors;
using Posting.Core.Interfaces.Asp;
using Posting.Infrastructure.Operations;

namespace Posting.API.Configuration
{
    public class ValidationBehaviour<TRequest, TOperationResult> : IPipelineBehavior<TRequest, IOperationResult<TOperationResult>>
    {
        private readonly IValidator<TRequest> _validator;

        public ValidationBehaviour(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public async Task<IOperationResult<TOperationResult>> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<IOperationResult<TOperationResult>> next)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                return await next().ConfigureAwait(false);
            }

            var resultBuilder = ResultBuilder
                .Error<TOperationResult>(ErrorCodes.BadArgument, "One or more validation errors have occured")
                .ForTarget(nameof(request));

            foreach (var validationResultError in validationResult.Errors)
            {
                resultBuilder.WithDetailsError(() =>
                    new ErrorBuilder(validationResultError.PropertyName, validationResultError.ErrorMessage).ForTarget(
                        validationResultError.PropertyName));
            }

            return resultBuilder.Build();
        }
    }
}