namespace VacationPlanner.Interfaces
{
    public interface IVacationDurationService
    {
        Task SetGlobalVacationDurationAsync(int days);
        Task SetVacationDurationByPositionAsync(int positionId, int days);
        Task<int> GetVacationDurationForUserAsync(string userId);
    }
}
