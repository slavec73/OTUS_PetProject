using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces;
using VacationPlanner.Models;

namespace VacationPlanner.Implementation.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(MapToDto(user, roles));
            }

            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return MapToDto(user, roles);
        }

        public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(string roleName)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            var userDtos = new List<UserDto>();

            foreach (var user in usersInRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(MapToDto(user, roles));
            }

            return userDtos;
        }

        public async Task<bool> ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return false;

            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            return addResult.Succeeded;
        }

        public async Task<bool> AddRolesToUserAsync(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRolesAsync(user, roles);
            return result.Succeeded;
        }

        public async Task<bool> RemoveRolesFromUserAsync(string userId, IEnumerable<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Enumerable.Empty<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null;
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