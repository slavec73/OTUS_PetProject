using VacationPlanner.Models.Requests;
using VacationPlanner.Models.Responses;

namespace VacationPlanner.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}

