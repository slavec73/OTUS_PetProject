using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Implementation.Repository;

public class EfVacationApprovalRepository : IVacationApprovalRepository
{
    private readonly ApplicationDbContext _context;

    public EfVacationApprovalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VacationApproval>> GetByRequestIdAsync(Guid requestId)
    {
        return await _context.VacationApprovals
            .Where(a => a.VacationRequestId == requestId)
            .OrderBy(a => a.ApprovalStage)
            .ToListAsync();
    }

    public async Task AddAsync(VacationApproval approval)
    {
        await _context.VacationApprovals.AddAsync(approval);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
