using FluentValidation;
using Posting.Infrastructure.Helpers;

namespace Posting.Infrastructure.Commands
{
    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator()
        {
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Url).NotEmpty().MaximumLength(100).MustBeLink();
            RuleFor(x => x.BlogId).NotEmpty().MaximumLength(36);
        }
    }
}