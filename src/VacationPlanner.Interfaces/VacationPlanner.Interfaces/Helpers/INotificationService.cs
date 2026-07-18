using VacationPlanner.Core.Notifications;

namespace VacationPlanner.Interfaces.Helpers
{
    public interface INotificationService
    {
        Task SendAsync(NotificationMessage message);
    }
}
