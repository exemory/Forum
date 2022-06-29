using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class PostCreationDtoValidator : AbstractValidator<PostCreationDto>
    {
        public PostCreationDtoValidator()
        {
            RuleFor(p => p.Content)
                .NotEmpty()
                .MaximumLength(10000);
            
            RuleFor(p => p.ThreadId)
                .NotEmpty();
        }
    }
}