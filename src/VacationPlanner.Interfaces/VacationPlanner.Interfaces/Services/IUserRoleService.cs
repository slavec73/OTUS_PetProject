namespace VacationPlanner.Interfaces.Services
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto> GetUserByIdAsync(Guid userId);

        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(Guid roleId);

        Task<bool> ChangeUserRoleAsync(Guid userId, Guid roleId);

        Task<IEnumerable<string>> GetAllRolesAsync();

        Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);

        Task<bool> UserExistsAsync(Guid userId);
    }

    public record UserDto
    {
        public required string Id { get; init; }
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required IEnumerable<string> Roles { get; init; }
    }
}
