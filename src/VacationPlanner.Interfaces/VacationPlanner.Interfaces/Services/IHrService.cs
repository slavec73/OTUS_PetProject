using VacationPlanner.Models.Enums;

namespace VacationPlanner.Interfaces.Services
{
    public interface IHrService
    {
        Task<IEnumerable<HrVacationRequestDto>> GetAllRequestsAsync();
        Task<IEnumerable<HrVacationRequestDto>> GetRequestsByPositionAsync(int positionId);
        Task<IEnumerable<HrVacationRequestDto>> GetRequestsByUserAsync(Guid userId);
        Task<IEnumerable<HrVacationRequestDto>> GetRequestsByStatusAsync(VacationRequestStatus status);

        Task<HrVacationRequestDto> ApproveRequestAsync(Guid requestId, Guid hrUserId, string? comment);
        Task<HrVacationRequestDto> ReturnForRevisionAsync(Guid requestId, Guid hrUserId, string? comment);

        Task<VacationDto> CreateVacationFromRequestAsync(Guid requestId, Guid hrUserId);

        Task<IEnumerable<VacationDto>> GetAllVacationsAsync();
        Task<IEnumerable<VacationDto>> GetVacationsByDateRangeAsync(DateTime from, DateTime to);
    }

    public record HrVacationRequestDto(
        Guid Id,
        Guid UserId,
        string? UserEmail,
        string? UserName,
        int? PositionId,
        string? PositionName,
        string Reason,
        DateTime DateFrom,
        DateTime DateTo,
        int TotalDays,
        string Status,
        DateTime CreatedAt,
        string? Comment);
}
