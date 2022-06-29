using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class ThreadCreationDtoValidator : AbstractValidator<ThreadCreationDto>
    {
        public ThreadCreationDtoValidator()
        {
            RuleFor(t => t.Topic)
                .NotEmpty()
                .Length(10, 200);
        }
    }
}