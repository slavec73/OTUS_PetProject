using VacationPlanner.Core.Events;

namespace VacationPlanner.Common.Events
{
    /// <summary>
    /// Событие руководитель отклонил заявку на отпуск.
    /// </summary>
    public class VacationRequestRejectedByManagerEvent : IEvent
    {
        public Guid VacationRequestId { get; }

        public string EmployeeMail { get; }

        public string ManagerId { get; }

        public VacationRequestRejectedByManagerEvent(
            Guid vacationRequestId,
            string employeeMail,
            string managerId)
        {
            VacationRequestId = vacationRequestId;
            EmployeeMail = employeeMail;
            ManagerId = managerId;
        }
    }
}
