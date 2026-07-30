using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Enums;

namespace VacationPlanner.Interfaces.Repository;

public interface IVacationRequestRepository
{
    Task<VacationRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<VacationRequest>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<VacationRequest>> GetPendingApprovalsForApproverAsync(Guid approverUserId);
    Task<IEnumerable<VacationRequest>> GetAllAsync();
    Task<IEnumerable<VacationRequest>> GetByPositionIdAsync(int positionId);
    Task<IEnumerable<VacationRequest>> GetByStatusAsync(VacationRequestStatus status);
    Task AddAsync(VacationRequest request);
    void Update(VacationRequest request);
    Task SaveChangesAsync();
}
