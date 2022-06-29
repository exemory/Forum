using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class SignUpDtoValidator : AbstractValidator<SignUpDto>
    {
        public SignUpDtoValidator()
        {
            RuleFor(d => d.Username)
                .NotEmpty()
                .Length(3, 15);

            RuleFor(d => d.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(d => d.Name)
                .MaximumLength(20);

            RuleFor(d => d.Password)
                .NotNull()
                .MinimumLength(8);
        }
    }
}