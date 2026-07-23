using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Enums;

namespace VacationPlanner.Implementation.Repository;

public class EfVacationRequestRepository : IVacationRequestRepository
{
    private readonly ApplicationDbContext _context;

    public EfVacationRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VacationRequest?> GetByIdAsync(Guid id)
    {
        return await _context.VacationRequests
            .Include(vr => vr.User)
            .Include(vr => vr.Approvals)
            .FirstOrDefaultAsync(vr => vr.VacationRequestId == id);
    }

    public async Task<IEnumerable<VacationRequest>> GetByUserIdAsync(Guid userId)
    {
        return await _context.VacationRequests
            .Where(vr => vr.UserId == userId)
            .OrderByDescending(vr => vr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<VacationRequest>> GetPendingApprovalsForApproverAsync(Guid approverUserId)
    {
        return await _context.VacationApprovals
            .Where(a => a.ApproverUserId == approverUserId
                     && a.Decision == VacationRequestStatus.PendingFirstApproval
                     || a.Decision == VacationRequestStatus.PendingSecondApproval)
            .Select(a => a.VacationRequest!)
            .Distinct()
            .ToListAsync();
    }

    public async Task AddAsync(VacationRequest request)
    {
        await _context.VacationRequests.AddAsync(request);
    }

    public void Update(VacationRequest request)
    {
        _context.VacationRequests.Update(request);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
