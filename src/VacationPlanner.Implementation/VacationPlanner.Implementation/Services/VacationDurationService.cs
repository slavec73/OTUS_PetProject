using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;

namespace VacationPlanner.Implementation.Services
{
    public class VacationDurationService : IVacationDurationService
    {
        private readonly IVacationDurationRepository _repository;
        private readonly IPositionRepository _positionRepository;
        private readonly IUserRoleService _userRoleService;

        public VacationDurationService(
            IVacationDurationRepository repository,
            IPositionRepository positionRepository,
            IUserRoleService userRoleService)
        {
            _repository = repository;
            _positionRepository = positionRepository;
            _userRoleService = userRoleService;
        }

        public async Task SetGlobalVacationDurationAsync(int days)
        {
            await _repository.SetGlobalSettingAsync(days);
            await _repository.SaveChangesAsync();
        }

        public async Task SetVacationDurationByPositionAsync(int positionId, int days)
        {
            var positionExists = await _positionRepository.ExistsAsync(positionId);
            if (!positionExists)
                throw new System.ArgumentException("Должность не найдена");

            await _repository.SetSettingForPositionAsync(positionId, days);
            await _repository.SaveChangesAsync();
        }

        public async Task<int> GetVacationDurationForUserAsync(Guid userId)
        {
            var user = await _userRoleService.GetUserByIdAsync(userId);
            if (user == null) return 20;

            var global = await _repository.GetGlobalSettingAsync();
            return global.DefaultVacationDays;
        }

        public Task<int> GetVacationDurationForUserAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
