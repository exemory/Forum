using System.Text.RegularExpressions;
using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class UserRoleUpdateDtoValidator : AbstractValidator<UserRoleUpdateDto>
    {
        public UserRoleUpdateDtoValidator()
        {
            RuleFor(u => u.Role)
                .NotNull()
                .Matches("User|Moderator", RegexOptions.IgnoreCase)
                .WithMessage($"'{nameof(UserRoleUpdateDto.Role)}' must be either 'User' or 'Moderator'");
        }
    }
}