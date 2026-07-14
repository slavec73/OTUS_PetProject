using FluentValidation;
using VacationPlanner.Models.Requests;

namespace VacationPlanner.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email обязателен")
                .EmailAddress()
                .WithMessage("Некорректный формат Email");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Пароль обязателен")
                .Length(6, 20)
                .WithMessage("Пароль должен содержать от 6 до 20 символов")
                .Matches("[A-Z]")
                .WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches("[a-z]")
                .WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches("[0-9]")
                .WithMessage("Пароль должен содержать хотя бы одну цифру")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Пароль должен содержать хотя бы один специальный символ");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Имя обязательно")
                .MaximumLength(100)
                .WithMessage("Имя не должно превышать 100 символов");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Фамилия обязательна")
                .MaximumLength(100)
                .WithMessage("Фамилия не должна превышать 100 символов");
        }
    }
}
