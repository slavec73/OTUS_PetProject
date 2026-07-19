using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class VacationRequestSubmittedNotificationHandler
    : IEventHandler<VacationRequestSubmittedEvent>
    {
        private readonly INotificationService _notificationService;

        public VacationRequestSubmittedNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(VacationRequestSubmittedEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
            {
                domainEvent.ManagerMail
            },

                Subject = Subjects.SubjectCreatedVacationRequest,

                Body =
               string.Format(
                Messages.VacationRequestSubmittedMessage,
                domainEvent.VacationRequestId),
            };


            await _notificationService.SendAsync(message);

        }
    }
}
