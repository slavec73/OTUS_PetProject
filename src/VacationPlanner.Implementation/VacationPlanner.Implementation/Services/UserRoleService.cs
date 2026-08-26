using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacationPlanner.Core.Events;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Responses;

namespace VacationPlanner.Implementation.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRoleService> _logger;
        private readonly IEventDispatcher _eventDispatcher;

        public UserRoleService(ApplicationDbContext context, ILogger<UserRoleService> logger, IEventDispatcher eventDispatcher)
        {
            _context = context;
            _logger = logger;
            _eventDispatcher = eventDispatcher;
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
            _logger.LogInformation("Start Get User by Id");
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                _logger.LogWarning($"user with id: {userId} not found");
                return null;
            }

            _logger.LogInformation("End Get User by Id");

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

        public async Task<ChangeUserPropertiesResponse> ChangeUserRoleAsync(Guid userId, Guid roleId)
        {
            _logger.LogInformation("Start Change User Role");
            var response = new ChangeUserPropertiesResponse();
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                _logger.LogWarning($"user with id: {userId} not found");
                response.Success = false;
                response.Message = $"user with id: {userId} not found";
                return response;
            }
            if (user.DepartmentId is null)
            {
                _logger.LogWarning($"user with id: {userId} not accepted to any department");
                response.Success = false;
                response.Message = $"user with id: {userId} not accepted to any department";
                return response;
            }

            var newRole = await _context.Roles
                .FirstOrDefaultAsync(x => x.RoleId == roleId);

            if (newRole is null)
            {
                _logger.LogWarning($"role with id: {roleId} not found");
                response.Success = false;
                response.Message = $"role with id: {roleId} not found";
                return response;
            }

            user.RoleId = roleId;

            await _context.SaveChangesAsync();

            await _eventDispatcher.PublishAsync(
                new ChangeUserRoleEvent(
                    user.Email,
                    newRole.Name));
            _logger.LogInformation("End Change User Role");
            response.Success = true;
            return response;
        }

        public async Task<ChangeUserPropertiesResponse> ChangeUserDepartmentAsync(Guid userId, int departmentId)
        {
            _logger.LogInformation("Start Change User Role");
            var response = new ChangeUserPropertiesResponse();
            var user = await _context.Users
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
            {
                _logger.LogWarning($"user with id: {userId} not found");
                response.Success = false;
                response.Message = $"user with id: {userId} not found";
                return response;
            }
            var newDeparment = await _context.Departments
                .FirstOrDefaultAsync(x => x.DepartmentId == departmentId);

            if (newDeparment is null)
            {
                _logger.LogWarning($"deparment with id: {departmentId} not found");
                response.Success = false;
                response.Message = $"deparment with id: {departmentId} not found";
                return response;
            }
            var @event = new ChangeUserDepartmentEvent(
                    user.Email,
                    user.Department?.Name,
                    newDeparment.Name);
            user.DepartmentId = departmentId;

            await _context.SaveChangesAsync();

            await _eventDispatcher.PublishAsync(@event);
            _logger.LogInformation("End Change User Role");
            response.Success = true;
            return response;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Select(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
        {
            _logger.LogInformation($"Start Get User Roles");
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || user.Role == null)
            {
                if (user == null)
                    _logger.LogWarning($"user with id: {userId} not found");
                if (user.Role == null)
                    _logger.LogWarning($"user with id: {userId} doesn't have role");
                return Enumerable.Empty<string>();
            }

            _logger.LogInformation($"End Get User Roles");
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