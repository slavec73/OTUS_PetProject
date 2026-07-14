namespace VacationPlanner.Interfaces.Services
{
    public interface IPositionService
    {
        Task<IEnumerable<PositionDto>> GetAllPositionsAsync();
        Task<PositionDto> GetPositionByIdAsync(int id);
        Task<PositionDto> CreatePositionAsync(CreatePositionDto dto);
        Task<PositionDto> UpdatePositionAsync(int id, UpdatePositionDto dto);
        Task<bool> DeletePositionAsync(int id);
    }

    public record PositionDto(int Id, string Name, string Description);
    public record CreatePositionDto(string Name, string Description);
    public record UpdatePositionDto(string Name, string Description);
}
