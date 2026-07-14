namespace VacationPlanner.Interfaces.Infrastructure
{
    public interface IDbHealthService
    {
        Task<bool> CanConnectAsync();
    }
}
