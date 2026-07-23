using FluentValidation;
using VacationPlanner.Models.Requests;

namespace VacationPlanner.Validators;

public class CreateVacationRequestValidator : AbstractValidator<CreateVacationRequest>
{
    public CreateVacationRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Причина обязательна")
            .MaximumLength(500).WithMessage("Причина не должна превышать 500 символов");

        RuleFor(x => x.DateFrom)
            .NotEmpty().WithMessage("Дата начала обязательна")
            .LessThanOrEqualTo(x => x.DateTo).WithMessage("Дата начала должна быть не позже даты окончания")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Дата начала не может быть в прошлом");

        RuleFor(x => x.DateTo)
            .NotEmpty().WithMessage("Дата окончания обязательна")
            .GreaterThanOrEqualTo(x => x.DateFrom).WithMessage("Дата окончания должна быть не раньше даты начала");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Комментарий не должен превышать 500 символов");
    }
}
