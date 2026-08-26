using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class ChangeUserRoleNotificationHandler
        : IEventHandler<ChangeUserRoleEvent>
    {
        private readonly INotificationService _notificationService;

        public ChangeUserRoleNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(
            ChangeUserRoleEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
           {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.ChangeUserRoleSubject,

                Body =
               string.Format(
    Messages.ChangeUserRoleMessage,
    domainEvent.RoleName),
            };


            await _notificationService.SendAsync(message);
        }
    }
}
