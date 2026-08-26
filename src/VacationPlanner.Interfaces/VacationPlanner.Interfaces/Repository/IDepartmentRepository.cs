namespace VacationPlanner.Interfaces.Repository
{
    public interface IDepartmentRepository
    {
        Task<Guid> GetManagerIdByDepartmentIdAsync(int departmentId);
    }
}
