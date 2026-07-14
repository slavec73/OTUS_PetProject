using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Interfaces.Repository
{
    public interface IRoleRepository
    {
        Task<Role> FindRoleByNameAsync(string roleName);
        Task<Role> FindRoleByIdAsync(Guid roleId);
    }
}
