using Blog.API.Infrastructure.Data.Models;
using FluentValidation;

namespace Blog.API.Infrastructure.Validators
{
    public class BlogControllerValidators
    {
        public class CreateValidator : AbstractValidator<BlogModel>
        {
            public CreateValidator()
            {
                RuleFor(x => x.Name).MinimumLength(3).MaximumLength(200);
                RuleFor(x => x.Path).MinimumLength(3).MaximumLength(200);
                RuleFor(x => x.Title).MinimumLength(3).MaximumLength(200);
                RuleFor(x => x.Uri).MinimumLength(5).MaximumLength(200);
            }
        }

        public class UpdateValidator : AbstractValidator<BlogModel>
        {
            public UpdateValidator()
            {
                RuleFor(x => x.Name).MinimumLength(3).MaximumLength(200);
                RuleFor(x => x.Path).MinimumLength(3).MaximumLength(200);
                RuleFor(x => x.Title).MinimumLength(3).MaximumLength(200);
                RuleFor(x => x.Uri).MinimumLength(5).MaximumLength(200);
            }
        }
    }
}