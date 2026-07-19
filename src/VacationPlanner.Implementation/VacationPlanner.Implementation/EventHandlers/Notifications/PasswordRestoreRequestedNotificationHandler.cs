using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class PasswordRestoreRequestedNotificationHandler
    : IEventHandler<PasswordRestoreRequestedEvent>
    {
        private readonly INotificationService _notificationService;


        public PasswordRestoreRequestedNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        public async Task HandleAsync(
            PasswordRestoreRequestedEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
                {
                domainEvent.Email
            },

                Body = string.Format(
                    Messages.RestorePasswordMessage,
                    domainEvent.Code),

                Subject = Subjects.RestorePasswordSubject
            };


            await _notificationService.SendAsync(message);
        }
    }
}
