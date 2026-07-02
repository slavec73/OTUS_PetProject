using VacationPlanner.Models;

namespace VacationPlanner.Interfaces
{
    public interface IVacationDurationRepository
    {
        Task<GlobalVacationSetting> GetGlobalSettingAsync();
        Task SetGlobalSettingAsync(int days);
        Task<PositionVacationSetting> GetSettingByPositionIdAsync(int positionId);
        Task SetSettingForPositionAsync(int positionId, int days);
        Task SaveChangesAsync();
    }
}
