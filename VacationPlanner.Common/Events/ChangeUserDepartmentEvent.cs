namespace VacationPlanner.Core.Events
{
    public class ChangeUserDepartmentEvent : IEvent
    {
        public string EmployeeMail { get; }

        public string OldDepartmentName { get; }

        public string NewDepartmentName { get; }

        public ChangeUserDepartmentEvent(
            string employeeMail,
            string oldDepartmentName,
            string newDepartmentName)
        {
            EmployeeMail = employeeMail;
            OldDepartmentName = oldDepartmentName;
            NewDepartmentName = newDepartmentName;
        }
    }
}
