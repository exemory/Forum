using System.Text.RegularExpressions;
using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class AccountUpdateDtoValidator : AbstractValidator<AccountUpdateDto>
    {
        public AccountUpdateDtoValidator()
        {
            RuleFor(d => d.Username)
                .NotEmpty()
                .Length(3, 15);

            RuleFor(d => d.Email)
                .NotNull()
                .EmailAddress();

            RuleFor(d => d.Name)
                .Matches("^[a-z ]*$", RegexOptions.IgnoreCase)
                .WithMessage($"'{nameof(AccountUpdateDto.Name)}' must consist only of latin letters and whitespace")
                .Length(1, 20);

            RuleFor(d => d.CurrentPassword)
                .NotNull();
        }
    }
}