using System.Text.Json;
using VacationPlanner.Core.Events;
using VacationPlanner.Interfaces.Helpers;
using VacationPlanner.Interfaces.Infrastructure;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Requests;
using VacationPlanner.Models.Responses;

namespace VacationPlanner.Implementation.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEventDispatcher _eventDispatcher;

        public AuthService(
            IJwtService jwtService,
            ICacheService cacheService,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IEventDispatcher eventDispatcher)
        {
            _jwtService = jwtService;
            _cacheService = cacheService;
            _userRepository = userRepository;
            _eventDispatcher = eventDispatcher;
            _roleRepository = roleRepository;
        }


        public async Task RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLower();

            var user = await _userRepository.FindUserByEmailAsync(email);

            if (user is not null)
            {
                throw new InvalidOperationException(
                    "Пользователь с таким Email уже существует");
            }


            var employeeRole = await _roleRepository.FindRoleByNameAsync(WellKnownRoles.Employee);


            if (employeeRole == null)
            {
                throw new InvalidOperationException(
                    "Роль Employee не найдена");
            }


            var createdUser = new User
            {
                UserId = Guid.NewGuid(),

                Email = email,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Password),

                FirstName = request.FirstName.Trim(),

                LastName = request.LastName.Trim(),

                RegistrationDate = DateTime.UtcNow,

                IsActive = true,

                RoleId = employeeRole.RoleId
            };

            await _userRepository.AddUserAsync(createdUser);

            await _eventDispatcher.PublishAsync(
                new UserRegisteredEvent(
                    createdUser.UserId,
                    createdUser.Email,
                    createdUser.FirstName));
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLower();


            var user = await _userRepository.FindUserByEmailAsync(email);


            if (user == null)
            {
                throw new InvalidOperationException(
                    "Неверный email или пароль");
            }


            var passwordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);


            if (!passwordValid)
            {
                throw new InvalidOperationException(
                    "Неверный email или пароль");
            }


            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    "Пользователь заблокирован");
            }

            var role = await _roleRepository.FindRoleByIdAsync(user.RoleId);

            var accessToken = _jwtService.GenerateToken(user, role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            await _cacheService.SetAsync(
                $"refresh_token:{refreshToken}",
                user.UserId.ToString(),
                TimeSpan.FromDays(7));

            return new LoginResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.Name,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            var key = $"refresh_token:{refreshToken}";


            var userIdValue = await _cacheService.GetAsync(key);


            if (string.IsNullOrEmpty(userIdValue))
            {
                throw new InvalidOperationException(
                    "Refresh token недействителен");
            }


            var userId = Guid.Parse(userIdValue);


            var user = await _userRepository.FindUserByIdAsync(userId);


            if (user == null)
            {
                throw new InvalidOperationException(
                    "Пользователь не найден");
            }


            // удаляем старый refresh token
            await _cacheService.RemoveAsync(key);

            var role = await _roleRepository.FindRoleByIdAsync(user.RoleId);
            var newAccessToken = _jwtService.GenerateToken(user, role);

            var newRefreshToken = _jwtService.GenerateRefreshToken();


            await _cacheService.SetAsync(
                $"refresh_token:{newRefreshToken}",
                user.UserId.ToString(),
                TimeSpan.FromDays(7));


            return new LoginResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                Role = user.Role.Name,

                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.FindUserByIdAsync(userId);


            if (user == null)
            {
                throw new InvalidOperationException(
                    "Пользователь не найден");
            }


            var passwordValid = BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.PasswordHash);


            if (!passwordValid)
            {
                throw new InvalidOperationException(
                    "Текущий пароль неверный");
            }


            if (request.CurrentPassword == request.NewPassword)
            {
                throw new InvalidOperationException(
                    "Новый пароль должен отличаться от текущего");
            }


            await _userRepository.ChangeUserPasswordAsync(userId, BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword));

            await _eventDispatcher.PublishAsync(
                new PasswordChangedEvent(
                    user.UserId,
                    user.Email,
                    user.FirstName));
        }

        public async Task ForgotPasswordAsync(
    ForgotPasswordRequest request)
        {
            var email = request.Email
                .Trim()
                .ToLower();


            var user = await _userRepository.FindUserByEmailAsync(email);


            // Не говорим пользователю существует email или нет
            if (user == null)
                return;


            var code = Random.Shared
                .Next(100000, 999999)
                .ToString();


            var resetData = new PasswordResetData
            {
                UserId = user.UserId,
                Code = code
            };


            await _cacheService.SetAsync(
                $"password_reset:{email}",
                JsonSerializer.Serialize(resetData),
                TimeSpan.FromMinutes(15));

            await _eventDispatcher.PublishAsync(
                new PasswordRestoreRequestedEvent(
                    user.UserId,
                    user.Email,
                    code));
        }

        public async Task ResetPasswordAsync(
    ResetPasswordRequest request)
        {
            var email = request.Email
                .Trim()
                .ToLower();


            var dataJson = await _cacheService.GetAsync(
                $"password_reset:{email}");


            if (string.IsNullOrEmpty(dataJson))
            {
                throw new InvalidOperationException(
                    "Код недействителен или истек");
            }


            var resetData =
                JsonSerializer.Deserialize<PasswordResetData>(
                    dataJson);


            if (resetData == null ||
                resetData.Code != request.Code)
            {
                throw new InvalidOperationException(
                    "Неверный код");
            }

            var user = await _userRepository.FindUserByIdAsync(resetData.UserId);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Пользователь не найден");
            }

            await _userRepository.ChangeUserPasswordAsync(user.UserId, BCrypt.Net.BCrypt.HashPassword(
                    request.NewPassword));


            await _cacheService.RemoveAsync(
                $"password_reset:{email}");

            await _eventDispatcher.PublishAsync(
                new PasswordChangedEvent(
                    user.UserId,
                    user.Email,
                    user.FirstName));
        }
    }
}
