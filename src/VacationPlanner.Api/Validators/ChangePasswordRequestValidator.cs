using FluentValidation;
using VacationPlanner.Models.Requests;

namespace VacationPlanner.Validators;

public class ChangePasswordRequestValidator
    : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Текущий пароль обязателен");


        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Новый пароль обязателен")
            .Length(6, 20)
            .WithMessage("Пароль должен содержать от 6 до 20 символов")
            .Matches("[A-Z]")
            .WithMessage("Пароль должен содержать заглавную букву")
            .Matches("[a-z]")
            .WithMessage("Пароль должен содержать строчную букву")
            .Matches("[0-9]")
            .WithMessage("Пароль должен содержать цифру")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage("Пароль должен содержать специальный символ");
    }
}