using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class VacationRequestApprovedByHrNotificationHandler
    : IEventHandler<VacationRequestApprovedByHrEvent>
    {
        private readonly INotificationService _notificationService;

        public VacationRequestApprovedByHrNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(
            VacationRequestApprovedByHrEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
            {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.SubjectVacationRequestApprovedByHr,

                Body =
                string.Format(
                Messages.RequestApprovedByHrMessage,
                domainEvent.VacationRequestId),
            };


            await _notificationService.SendAsync(message);
        }
    }
}
