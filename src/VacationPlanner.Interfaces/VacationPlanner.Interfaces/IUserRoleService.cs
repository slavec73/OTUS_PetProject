namespace VacationPlanner.Interfaces
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto> GetUserByIdAsync(string userId);

        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleName);

        Task<bool> ChangeUserRoleAsync(string userId, string newRole);

        Task<bool> AddRolesToUserAsync(string userId, IEnumerable<string> roles);

        Task<bool> RemoveRolesFromUserAsync(string userId, IEnumerable<string> roles);

        Task<IEnumerable<string>> GetAllRolesAsync();

        Task<IEnumerable<string>> GetUserRolesAsync(string userId);

        Task<bool> UserExistsAsync(string userId);
    }

    public record UserDto
    {
        public string Id { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public IEnumerable<string> Roles { get; init; }
    }
}
