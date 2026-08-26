namespace VacationPlanner.Core.Events
{
    public class ChangeUserDepartmentEvent : IEvent
    {
        public string EmployeeMail { get; }

        public string OldDepartmentName { get; }

        public string NewDepartmentName { get; }

        public string OldPositionName { get; }

        public string NewPositionName { get; }

        public ChangeUserDepartmentEvent(
            string employeeMail,
            string oldDepartmentName,
            string newDepartmentName,
            string oldPositionName,
            string newPositionName)
        {
            EmployeeMail = employeeMail;
            OldDepartmentName = oldDepartmentName;
            NewDepartmentName = newDepartmentName;
            OldPositionName = oldPositionName;
            NewPositionName = newPositionName;
        }
    }
}
