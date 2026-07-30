using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Implementation.Repository;

public class EfVacationRepository : IVacationRepository
{
    private readonly ApplicationDbContext _context;

    public EfVacationRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Vacation>> GetAllAsync()
    {
        return await _context.Vacations
            .Include(v => v.User)
            .OrderByDescending(v => v.DateFrom)
            .ToListAsync();
    }
    public async Task<IEnumerable<Vacation>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.Vacations
            .Include(v => v.User)
            .Where(v => v.DateFrom >= from && v.DateTo <= to)
            .OrderByDescending(v => v.DateFrom)
            .ToListAsync();
    }
    public async Task<IEnumerable<Vacation>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Vacations
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.DateFrom)
            .ToListAsync();
    }

    public async Task<Vacation?> GetByIdAsync(Guid id)
    {
        return await _context.Vacations.FindAsync(id);
    }

    public async Task AddAsync(Vacation vacation)
    {
        await _context.Vacations.AddAsync(vacation);
    }

    public void Update(Vacation vacation)
    {
        _context.Vacations.Update(vacation);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
