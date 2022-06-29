﻿using FluentValidation;
using Service.DataTransferObjects;

namespace Service.Validators
{
    public class ThreadUpdateDtoValidator : AbstractValidator<ThreadUpdateDto>
    {
        public ThreadUpdateDtoValidator()
        {
            RuleFor(t => t.Topic)
                .NotEmpty()
                .Length(10, 200);
        }
    }
}