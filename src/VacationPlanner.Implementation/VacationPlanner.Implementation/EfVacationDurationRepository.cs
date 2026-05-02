using Microsoft.EntityFrameworkCore;
using VacationPlanner.Interfaces;
using VacationPlanner.Models;
using VacationPlanner.Data;

public class EfVacationDurationRepository : IVacationDurationRepository
{
    private readonly ApplicationDbContext _context;
    public EfVacationDurationRepository(ApplicationDbContext context) => _context = context;

    public async Task<GlobalVacationSetting> GetGlobalSettingAsync()
    {
        return await _context.GlobalVacationSettings.FirstOrDefaultAsync()
               ?? new GlobalVacationSetting { DefaultVacationDays = 20 };
    }

    public async Task SetGlobalSettingAsync(int days)
    {
        var setting = await _context.GlobalVacationSettings.FirstOrDefaultAsync();
        if (setting == null)
        {
            setting = new GlobalVacationSetting { DefaultVacationDays = days };
            await _context.GlobalVacationSettings.AddAsync(setting);
        }
        else
        {
            setting.DefaultVacationDays = days;
            _context.GlobalVacationSettings.Update(setting);
        }
    }

    public async Task<PositionVacationSetting> GetSettingByPositionIdAsync(int positionId)
    {
        return await _context.PositionVacationSettings
            .FirstOrDefaultAsync(pvs => pvs.PositionId == positionId);
    }

    public async Task SetSettingForPositionAsync(int positionId, int days)
    {
        var existing = await GetSettingByPositionIdAsync(positionId);
        if (existing == null)
        {
            var newSetting = new PositionVacationSetting
            {
                PositionId = positionId,
                VacationDays = days
            };
            await _context.PositionVacationSettings.AddAsync(newSetting);
        }
        else
        {
            existing.VacationDays = days;
            _context.PositionVacationSettings.Update(existing);
        }
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
