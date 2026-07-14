using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Implementation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeUserPasswordAsync(Guid userId, string newPassword)
        {
            var user = await FindUserByIdAsync(userId);
            if (user is not null)
            {
                user.PasswordHash = newPassword;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users
                 .FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                throw;
            }

        }

        public async Task<User> FindUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
