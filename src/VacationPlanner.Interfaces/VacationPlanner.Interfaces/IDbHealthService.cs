namespace VacationPlanner.Interfaces
{
    public interface IDbHealthService
    {
        Task<bool> CanConnectAsync();
    }
}
