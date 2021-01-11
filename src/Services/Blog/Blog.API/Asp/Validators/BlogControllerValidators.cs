using Blog.API.Dtos;
using FluentValidation;

namespace Blog.API.Asp.Validators
{
    public class BlogControllerValidators
    {
        public class CreateValidator : AbstractValidator<BlogCreateDto>
        {
            public CreateValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
                RuleFor(x => x.Path).NotEmpty().MaximumLength(200);
                RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
                RuleFor(x => x.Uri).NotEmpty().MaximumLength(200);
            }
        }

        public class UpdateValidator : AbstractValidator<BlogUpdateDto>
        {
            public UpdateValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
                RuleFor(x => x.Path).NotEmpty().MaximumLength(200);
                RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
                RuleFor(x => x.Uri).NotEmpty().MaximumLength(200);
            }
        }
    }
}