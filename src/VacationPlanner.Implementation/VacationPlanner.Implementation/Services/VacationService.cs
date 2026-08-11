using Microsoft.Extensions.Logging;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;


namespace VacationPlanner.Implementation.Services
{

    public class VacationService : IVacationService
    {
        private readonly IVacationRepository _vacationRepository;
        private readonly ILogger<VacationService> _logger;

        public VacationService(IVacationRepository vacationRepository, ILogger<VacationService> logger)
        {
            _vacationRepository = vacationRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<VacationDto>> GetMyVacationsAsync(Guid userId)
        {
            _logger.LogInformation("Start Get Vacations by UserId");
            var vacations = await _vacationRepository.GetByUserIdAsync(userId);
            _logger.LogInformation("End Get Vacations by UserId");
            return vacations.Select(v => new VacationDto(
                v.VacationId,
                v.DateFrom,
                v.DateTo,
                v.TotalDays,
                v.VacationType,
                v.IsPaid,
                v.CreatedAt));
        }
    }
}
