using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Interfaces.Helpers
{
    public interface IJwtService
    {
        string GenerateToken(User user, Role role);
        public string GenerateRefreshToken();
    }
}
