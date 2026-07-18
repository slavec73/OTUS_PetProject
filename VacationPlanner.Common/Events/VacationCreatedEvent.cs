using VacationPlanner.Core.Events;

namespace VacationPlanner.Common.Events
{
    /// <summary>
    /// Событие отпуск создан.
    /// </summary>
    public class VacationCreatedEvent : IEvent
    {
        public Guid VacationId { get; }

        public string EmployeeMail { get; }

        public string ManagerMail { get; }

        public VacationCreatedEvent(
            Guid vacationId,
            string employeeMail,
            string managerMail)
        {
            VacationId = vacationId;
            EmployeeMail = employeeMail;
            ManagerMail = managerMail;
        }
    }
}
