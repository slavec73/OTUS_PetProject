using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Interfaces.Repository
{
    public interface IPositionRepository
    {
        Task<IEnumerable<Position>> GetAllAsync();
        Task<Position> GetByIdAsync(int id);
        Task AddAsync(Position position);
        void Update(Position position);
        void Delete(Position position);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}