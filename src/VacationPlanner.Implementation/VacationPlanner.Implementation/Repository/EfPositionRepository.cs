using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Models.DbModels;

public class EfPositionRepository : IPositionRepository
{
    private readonly ApplicationDbContext _context;
    public EfPositionRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Position>> GetAllAsync() => await _context.Positions.ToListAsync();
    public async Task<Position> GetByIdAsync(int id) => await _context.Positions.FindAsync(id);
    public async Task AddAsync(Position position) => await _context.Positions.AddAsync(position);
    public void Update(Position position) => _context.Positions.Update(position);
    public void Delete(Position position) => _context.Positions.Remove(position);
    public async Task<bool> ExistsAsync(int id) => await _context.Positions.AnyAsync(p => p.Id == id);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}