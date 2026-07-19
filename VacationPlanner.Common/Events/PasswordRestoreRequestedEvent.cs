namespace VacationPlanner.Core.Events
{
    /// <summary>
    /// Событие отправлен запрос на сброс пароля.
    /// </summary>
    public class PasswordRestoreRequestedEvent : IEvent
    {
        public Guid UserId { get; }

        public string Email { get; }

        public string Code { get; }


        public PasswordRestoreRequestedEvent(
            Guid userId,
            string email,
            string code)
        {
            UserId = userId;
            Email = email;
            Code = code;
        }
    }
}
