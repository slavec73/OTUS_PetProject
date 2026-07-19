namespace VacationPlanner.Core.Events
{
    /// <summary>
    /// Событие пользователь зарегестрировался.
    /// </summary>
    public class UserRegisteredEvent : IEvent
    {
        public Guid UserId { get; }

        public string Email { get; }

        public string FirstName { get; }


        public UserRegisteredEvent(
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
