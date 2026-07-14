using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User> FindUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<User> FindUserByIdAsync(Guid userId);

        Task ChangeUserPasswordAsync(Guid userId, string newPassword);
    }
}
