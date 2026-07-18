using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class VacationRequestApprovedByManagerNotificationHandler
    : IEventHandler<VacationRequestApprovedByManagerEvent>
    {
        private readonly INotificationService _notificationService;

        public VacationRequestApprovedByManagerNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(
            VacationRequestApprovedByManagerEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
           {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.SubjectVacationRequestApprovedByManager,

                Body =
               string.Format(
    Messages.RequestApprovedByManagerMessage,
    domainEvent.VacationRequestId),
            };


            await _notificationService.SendAsync(message);
        }
    }
}
