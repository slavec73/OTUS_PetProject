using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class PasswordChangedNotificationHandler
    : IEventHandler<PasswordChangedEvent>
    {
        private readonly INotificationService _notificationService;


        public PasswordChangedNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        public async Task HandleAsync(
            PasswordChangedEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
                {
                domainEvent.Email
            },

                Body = string.Format(
                    Messages.SuccessChangePasswordMessage,
                    domainEvent.FirstName),

                Subject = Subjects.ChangePasswordSubject
            };


            await _notificationService.SendAsync(message);
        }
    }
}
