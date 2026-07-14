using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Implementation.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Role> FindRoleByIdAsync(Guid roleId)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == roleId);
        }

        public async Task<Role> FindRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x => x.Name == roleName);
        }
    }
}
