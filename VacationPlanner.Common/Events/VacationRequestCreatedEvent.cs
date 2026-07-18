using VacationPlanner.Core.Events;

namespace VacationPlanner.Common.Events
{
    /// <summary>
    /// Событие заявка на отпуск создана.
    /// </summary>
    public class VacationRequestCreatedEvent : IEvent
    {
        public Guid VacationRequestId { get; }

        public string EmployeeMail { get; }

        public VacationRequestCreatedEvent(
            Guid vacationRequestId,
            string employeeMail)
        {
            VacationRequestId = vacationRequestId;
            EmployeeMail = employeeMail;
        }
    }
}
