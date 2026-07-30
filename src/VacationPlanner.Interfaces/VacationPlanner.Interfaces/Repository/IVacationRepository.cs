using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Interfaces.Repository;

public interface IVacationRepository
{
    Task<IEnumerable<Vacation>> GetByUserIdAsync(Guid userId);
    Task<Vacation?> GetByIdAsync(Guid id);
    Task<IEnumerable<Vacation>> GetAllAsync();
    Task<IEnumerable<Vacation>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task AddAsync(Vacation vacation);
    void Update(Vacation vacation);
    Task SaveChangesAsync();
}
