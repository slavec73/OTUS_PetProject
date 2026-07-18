using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class UserRegisteredNotificationHandler
    : IEventHandler<UserRegisteredEvent>
    {
        private readonly INotificationService _notificationService;


        public UserRegisteredNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        public async Task HandleAsync(
            UserRegisteredEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
                {
                domainEvent.Email
            },

                Body = string.Format(
                    Messages.SuccessRegistrationMessage,
                    domainEvent.FirstName),

                Subject = Subjects.RegistrationSubject
            };


            await _notificationService.SendAsync(message);
        }
    }
}
