using VacationPlanner.Core.Events;

namespace VacationPlanner.Common.Events
{
    /// <summary>
    /// Событие сотрудник отправил заявку на согласование
    /// </summary>
    public class VacationRequestSubmittedEvent : IEvent
    {
        public Guid VacationRequestId { get; }

        public string EmployeeMail { get; }

        public string ManagerMail { get; }

        public VacationRequestSubmittedEvent(
            Guid vacationRequestId,
            string employeeMail,
            string managerMail)
        {
            VacationRequestId = vacationRequestId;
            EmployeeMail = employeeMail;
            ManagerMail = managerMail;
        }
    }
}
