namespace VacationPlanner.Models.Requests
{
    public class PasswordResetData
    {
        public Guid UserId { get; set; }

        public string Code { get; set; } = null!;
    }
}
