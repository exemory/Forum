using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        public SignInDtoValidator()
        {
            RuleFor(d => d.Login)
                .NotEmpty();
            
            RuleFor(d => d.Password)
                .NotNull();
        }
    }
}