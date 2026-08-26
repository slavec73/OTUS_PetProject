using VacationPlanner.Core.Events;

namespace VacationPlanner.Common.Events
{
    /// <summary>
    /// Событие заявка согласована руководителем.
    /// </summary>
    public class VacationRequestApprovedByManagerEvent : IEvent
    {
        public Guid VacationRequestId { get; }

        public string EmployeeMail { get; }

        public string ManagerId { get; }

        public VacationRequestApprovedByManagerEvent(
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
