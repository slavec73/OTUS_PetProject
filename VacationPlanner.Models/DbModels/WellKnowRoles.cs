namespace VacationPlanner.Models.DbModels
{
    public static class WellKnownRoles
    {
        public static readonly Guid AdministratorId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static readonly Guid HrId =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static readonly Guid ManagerId =
            Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static readonly Guid EmployeeId =
            Guid.Parse("44444444-4444-4444-4444-444444444444");

        public const string Administrator = "Administrator";
        public const string Hr = "HR";
        public const string Manager = "Manager";
        public const string Employee = "Employee";
    }
}
