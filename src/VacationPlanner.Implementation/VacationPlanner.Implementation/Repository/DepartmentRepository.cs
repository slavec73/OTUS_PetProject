using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Implementation.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> GetManagerIdByDepartmentIdAsync(int departmentId)
        {
            return _context.Departments.First(x => x.DepartmentId == departmentId).ManagerId;
        }
    }
}
