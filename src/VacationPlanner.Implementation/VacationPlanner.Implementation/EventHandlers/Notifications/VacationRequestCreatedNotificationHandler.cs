using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class VacationRequestCreatedNotificationHandler
    : IEventHandler<VacationRequestCreatedEvent>
    {
        private readonly INotificationService _notificationService;

        public VacationRequestCreatedNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(VacationRequestCreatedEvent domainEvent)
        {
            var message = new NotificationMessage
            {
                RecipientMails = new[]
            {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.SubjectVacationRequestCreated,

                Body =
                string.Format(
            Messages.CreatedVacationRequestMessage,
            domainEvent.VacationRequestId),
            };


            await _notificationService.SendAsync(message);
        }
    }
}
