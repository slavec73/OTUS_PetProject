using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class VacationRequestRejectedByHrNotificationHandler
    : IEventHandler<VacationRequestRejectedByHrEvent>
    {
        private readonly INotificationService _notificationService;

        public VacationRequestRejectedByHrNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(
            VacationRequestRejectedByHrEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
            {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.SubjectVacationRejectedByHr,

                Body =
                string.Format(
    Messages.RequestRejectedByHrMessage,
    domainEvent.VacationRequestId),
            };


            await _notificationService.SendAsync(message);
        }
    }
}
