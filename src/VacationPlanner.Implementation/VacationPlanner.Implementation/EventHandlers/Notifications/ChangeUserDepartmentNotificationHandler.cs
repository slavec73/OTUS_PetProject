using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class ChangeUserDepartmentNotificationHandler
        : IEventHandler<ChangeUserDepartmentEvent>
    {
        private readonly INotificationService _notificationService;

        public ChangeUserDepartmentNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(
            ChangeUserDepartmentEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
           {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.ChangeUserDepartmentSubject,

                Body = string.IsNullOrEmpty(domainEvent.OldDepartmentName) ?
               string.Format(
    Messages.FirstAcceptToDeparmentMessage,
    domainEvent.NewPositionName,
    domainEvent.NewDepartmentName)
               : string.Format(
    Messages.ChangeDepartmentMessage,
    domainEvent.OldPositionName,
    domainEvent.OldDepartmentName,
    domainEvent.NewPositionName,
    domainEvent.NewDepartmentName)
            };

            await _notificationService.SendAsync(message);
        }
    }
}
