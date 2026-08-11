using Microsoft.Extensions.Logging;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;

namespace VacationPlanner.Implementation.Services
{
    public class VacationDurationService : IVacationDurationService
    {
        private readonly IVacationDurationRepository _repository;
        private readonly IPositionRepository _positionRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<VacationDurationService> _logger;

        public VacationDurationService(
            IVacationDurationRepository repository,
            IPositionRepository positionRepository,
            IUserRoleService userRoleService,
            ILogger<VacationDurationService> logger)
        {
            _repository = repository;
            _positionRepository = positionRepository;
            _userRoleService = userRoleService;
            _logger = logger;

        }

        public async Task SetGlobalVacationDurationAsync(int days)
        {
            _logger.LogInformation($"Start Set Global Vacation Duration");
            await _repository.SetGlobalSettingAsync(days);
            await _repository.SaveChangesAsync();
            _logger.LogInformation($"End Set Global Vacation Duration");
        }

        public async Task SetVacationDurationByPositionAsync(int positionId, int days)
        {
            _logger.LogInformation($"Start Set Vacation Duration By Position");
            var positionExists = await _positionRepository.ExistsAsync(positionId);
            if (!positionExists)
            {

                _logger.LogError($"Position with id: {positionId} not found");
                throw new System.ArgumentException("Должность не найдена");
            }

            await _repository.SetSettingForPositionAsync(positionId, days);
            await _repository.SaveChangesAsync();
            _logger.LogInformation($"End Set Vacation Duration By Position");
        }

        public async Task<int> GetVacationDurationForUserAsync(Guid userId)
        {
            _logger.LogInformation($"Start Get Vacation Duration By UserId");
            var user = await _userRoleService.GetUserByIdAsync(userId);
            if (user == null) return 20;

            var global = await _repository.GetGlobalSettingAsync();
            _logger.LogInformation($"End Get Vacation Duration By UserId");
            return global.DefaultVacationDays;
        }

        public Task<int> GetVacationDurationForUserAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
