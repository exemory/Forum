using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class PostUpdateDtoValidator : AbstractValidator<PostUpdateDto>
    {
        public PostUpdateDtoValidator()
        {
            RuleFor(p => p.Content)
                .NotEmpty()
                .MaximumLength(10000);
        }
    }
}