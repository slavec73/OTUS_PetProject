namespace VacationPlanner.Core.Events
{
    /// <summary>
    /// Событие пароль изменился.
    /// </summary>
    public class PasswordChangedEvent : IEvent
    {
        public Guid UserId { get; }

        public string Email { get; }

        public string FirstName { get; }


        public PasswordChangedEvent(
            Guid userId,
            string email,
            string firstName)
        {
            UserId = userId;
            Email = email;
            FirstName = firstName;
        }
    }
}
