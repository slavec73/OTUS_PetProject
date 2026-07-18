using VacationPlanner.Common.Events;
using VacationPlanner.Core.Events;
using VacationPlanner.Core.Notifications;
using VacationPlanner.Interfaces.Helpers;

namespace VacationPlanner.Implementation.EventHandlers.Notifications
{
    public class VacationCreatedNotificationHandler
    : IEventHandler<VacationCreatedEvent>
    {
        private readonly INotificationService _notificationService;

        public VacationCreatedNotificationHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task HandleAsync(VacationCreatedEvent domainEvent)
        {
            var employeeMessage = new NotificationMessage
            {
                RecipientMails = new[]
    {
                domainEvent.EmployeeMail
            },

                Subject = Subjects.SubjectVacationCreated,

                Body =
                string.Format(
                    Messages.CreatedVacationRequestMessage,
                    domainEvent.VacationId),
            };


            await _notificationService.SendAsync(employeeMessage);



            var managerMessage = new NotificationMessage
            {
                RecipientMails = new[]
                {
                domainEvent.ManagerMail
            },

                Subject = Subjects.SubjectCreatedVacationForManager,

                Body =
                    string.Format(
                    Messages.VacationCreatedForManagerMessage,
                    domainEvent.VacationId),
            };


            await _notificationService.SendAsync(managerMessage);

        }
    }
}
