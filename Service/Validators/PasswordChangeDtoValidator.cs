using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class PasswordChangeDtoValidator : AbstractValidator<PasswordChangeDto>
    {
        public PasswordChangeDtoValidator()
        {
            RuleFor(d => d.CurrentPassword)
                .NotNull();
            
            RuleFor(d => d.NewPassword)
                .NotNull()
                .MinimumLength(8);
        }
    }
}