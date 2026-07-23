using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;


namespace VacationPlanner.Implementation.Services
{

    public class VacationService : IVacationService
    {
        private readonly IVacationRepository _vacationRepository;

        public VacationService(IVacationRepository vacationRepository)
        {
            _vacationRepository = vacationRepository;
        }

        public async Task<IEnumerable<VacationDto>> GetMyVacationsAsync(Guid userId)
        {
            var vacations = await _vacationRepository.GetByUserIdAsync(userId);
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
