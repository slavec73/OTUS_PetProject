namespace VacationPlanner.Models.DbModels
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime RegistrationDate { get; set; }

        public bool IsActive { get; set; }

        public Guid RoleId { get; set; }

        public Role Role { get; set; } = null!;
    }
}
