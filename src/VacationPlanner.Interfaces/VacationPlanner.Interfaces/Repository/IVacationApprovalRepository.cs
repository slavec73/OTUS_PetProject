using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Interfaces.Repository;

public interface IVacationApprovalRepository
{
    Task<IEnumerable<VacationApproval>> GetByRequestIdAsync(Guid requestId);
    Task AddAsync(VacationApproval approval);
    Task SaveChangesAsync();
}
