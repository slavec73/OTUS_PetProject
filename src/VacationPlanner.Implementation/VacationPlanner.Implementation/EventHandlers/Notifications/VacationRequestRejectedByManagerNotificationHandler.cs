using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class VacationRequestRejectedByManagerNotificationHandler
    : IEventHandler<VacationRequestRejectedByManagerEvent>
    {
        private readonly INotificationService _notificationService;

        public VacationRequestRejectedByManagerNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(
            VacationRequestRejectedByManagerEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
            {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.SubjectVacationRejectedByManager,

                Body =
                string.Format(
                Messages.RequestApprovedByManagerMessage,
                domainEvent.VacationRequestId),
            };
        }
    }
}
