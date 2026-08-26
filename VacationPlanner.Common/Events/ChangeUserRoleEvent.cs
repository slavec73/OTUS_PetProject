namespace VacationPlanner.Core.Events
{
    public class ChangeUserRoleEvent : IEvent
    {
        public string EmployeeMail { get; }

        public string RoleName { get; }

        public ChangeUserRoleEvent(
            string employeeMail,
            string roleName)
        {
            EmployeeMail = employeeMail;
            RoleName = roleName;
        }
    }
}
