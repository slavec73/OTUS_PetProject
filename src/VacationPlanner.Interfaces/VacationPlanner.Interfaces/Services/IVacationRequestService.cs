namespace VacationPlanner.Interfaces.Services
{
    public interface IVacationRequestService
    {
        Task<VacationRequestDto?> GetByIdAsync(Guid id, Guid currentUserId);
        Task<IEnumerable<VacationRequestDto>> GetMyRequestsAsync(Guid userId);
        Task<IEnumerable<VacationRequestDto>> GetPendingApprovalsAsync(Guid approverUserId);
        Task<VacationRequestDto> CreateAsync(CreateVacationRequestDto dto, Guid userId);
        Task<VacationRequestDto> UpdateDraftAsync(Guid id, UpdateVacationRequestDto dto, Guid userId);
        Task<VacationRequestDto> SubmitForApprovalAsync(Guid id, Guid userId);
    }

    public record VacationRequestDto(
        Guid Id,
        string Reason,
        DateTime DateFrom,
        DateTime DateTo,
        int TotalDays,
        string Status,
        DateTime CreatedAt,
        string? Comment);

    public record CreateVacationRequestDto(
        string Reason,
        DateTime DateFrom,
        DateTime DateTo,
        string? Comment);

    public record UpdateVacationRequestDto(
        string Reason,
        DateTime DateFrom,
        DateTime DateTo,
        string? Comment);
}