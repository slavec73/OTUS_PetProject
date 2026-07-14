using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Implementation.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ApplicationDbContext _context;

        public UserRoleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(x => x.Role)
                .Select(x => new UserDto
                {
                    Id = x.UserId.ToString(),
                    Email = x.Email,
                    UserName = $"{x.FirstName} {x.LastName}",
                    Roles = new List<string>
                    {
                x.Role.Name
                    }
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.UserId.ToString(),
                Email = user.Email,
                UserName = $"{user.FirstName} {user.LastName}",
                Roles = new List<string>
                    {
                        user.Role.Name
                    }
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(Guid roleId)
        {
            return await _context.Users
                .Include(x => x.Role)
                .Where(x => x.Role.RoleId == roleId)
                .Select(x => new UserDto
                {
                    Id = x.UserId.ToString(),
                    Email = x.Email,
                    UserName = $"{x.FirstName} {x.LastName}",
                    Roles = new List<string>
                    {
                x.Role.Name
                    }
                })
                .ToListAsync();
        }

        public async Task<bool> ChangeUserRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                return false;

            user.RoleId = roleId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Select(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.Role == null)
                return Enumerable.Empty<string>();

            return new List<string>
            {
                user.Role.Name
            };
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId);
        }

        private UserDto MapToDto(ApplicationUser user, IList<string> roles)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };
        }
    }
}