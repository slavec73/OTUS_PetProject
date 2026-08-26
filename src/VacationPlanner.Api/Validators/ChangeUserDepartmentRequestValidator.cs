using FluentValidation;
using VacationPlanner.Models.Requests;

namespace VacationPlanner.Validators
{
    public class ChangeUserDepartmentRequestValidator : AbstractValidator<ChangeUserDepartmentRequest>
    {
        public ChangeUserDepartmentRequestValidator()
        {
            RuleFor(x => x.PositionId)
                .NotEmpty()
                .WithMessage("Идентификатор должности обязателен");

            RuleFor(x => x.DepartmentId)
                .NotEmpty()
                .WithMessage("Идентификатор департамента обязателен");
        }
    }
}
