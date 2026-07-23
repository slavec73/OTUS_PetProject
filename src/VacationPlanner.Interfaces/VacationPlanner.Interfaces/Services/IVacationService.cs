namespace VacationPlanner.Interfaces.Services
{
    public interface IVacationService
    {
        Task<IEnumerable<VacationDto>> GetMyVacationsAsync(Guid userId);
    }

    public record VacationDto(
        Guid Id,
        DateTime DateFrom,
        DateTime DateTo,
        int TotalDays,
        string VacationType,
        bool IsPaid,
        DateTime CreatedAt);
}