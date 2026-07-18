using VacationPlanner.Core.Events;

namespace VacationPlanner.Common.Events
{
    /// <summary>
    /// Событие HR отклонил заявку на отпуск.
    /// </summary>
    public class VacationRequestRejectedByHrEvent : IEvent
    {
        public Guid VacationRequestId { get; }

        public string EmployeeMail { get; }

        public string HrId { get; }

        public VacationRequestRejectedByHrEvent(
            Guid vacationRequestId,
            string employeeMail,
            string hrId)
        {
            VacationRequestId = vacationRequestId;
            EmployeeMail = employeeMail;
            HrId = hrId;
        }
    }
}
