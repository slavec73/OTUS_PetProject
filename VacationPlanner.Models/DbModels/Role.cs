namespace VacationPlanner.Models.DbModels
{
    public class Role
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
