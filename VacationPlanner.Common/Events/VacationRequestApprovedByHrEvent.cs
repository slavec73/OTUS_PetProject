using VacationPlanner.Core.Events;

namespace VacationPlanner.Common.Events
{
    /// <summary>
    /// Событие HR согласовал заявку на отпуск.
    /// </summary>
    public class VacationRequestApprovedByHrEvent : IEvent
    {
        public Guid VacationRequestId { get; }

        public Guid VacationId { get; }

        public string EmployeeMail { get; }

        public string HrId { get; }

        public VacationRequestApprovedByHrEvent(
            Guid vacationRequestId,
            Guid vacationId,
            string employeeMail,
            string hrId)
        {
            VacationRequestId = vacationRequestId;
            VacationId = vacationId;
            EmployeeMail = employeeMail;
            HrId = hrId;
        }
    }
}
