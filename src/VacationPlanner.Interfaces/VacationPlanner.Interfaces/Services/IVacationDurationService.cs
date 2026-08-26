namespace VacationPlanner.Interfaces.Services
{
    public interface IVacationDurationService
    {
        Task SetGlobalVacationDurationAsync(int days);
        Task SetVacationDurationByPositionAsync(int positionId, int days);
    }
}
