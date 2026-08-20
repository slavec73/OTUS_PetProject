using VacationPlanner.Models.Enums;

namespace VacationPlanner.Interfaces.Services
{
    /// <summary>
    /// Функциональность руководителя (менеджера) — урезанная версия функциональности HR.
    /// Менеджер работает только с заявками сотрудников своего подразделения.
    ///
    /// Жизненный цикл заявки:
    /// Draft (черновик, создал сотрудник)
    ///  → PendingFirstApproval (сотрудник отправил на согласование, 1-й этап — руководитель)
    ///  → руководитель согласовал → PendingSecondApproval (2-й этап — HR)
    ///  → руководитель вернул на доработку → Draft
    ///  → HR согласовал → Approved (отпуск можно оформить)
    ///  → HR вернул на доработку → Draft
    /// </summary>
    public interface IManagerService
    {
        /// <summary>Заявки сотрудников подразделения, ожидающие согласования руководителя (1-й этап).</summary>
        Task<IEnumerable<ManagerVacationRequestDto>> GetPendingRequestsAsync(Guid managerId);

        /// <summary>Все заявки конкретного сотрудника своего подразделения.</summary>
        Task<IEnumerable<ManagerVacationRequestDto>> GetRequestsByEmployeeAsync(Guid managerId, Guid employeeId);

        /// <summary>Заявки сотрудников подразделения по статусу.</summary>
        Task<IEnumerable<ManagerVacationRequestDto>> GetRequestsByStatusAsync(Guid managerId, VacationRequestStatus status);

        /// <summary>Конкретная заявка сотрудника своего подразделения, либо null если заявка не из подразделения.</summary>
        Task<ManagerVacationRequestDto?> GetByIdAsync(Guid requestId, Guid managerId);

        /// <summary>Согласовать заявку (1-й этап) — статус станет PendingSecondApproval (заявка уйдёт в HR).</summary>
        Task<ManagerVacationRequestDto> ApproveRequestAsync(Guid requestId, Guid managerId, string? comment);

        /// <summary>Вернуть заявку на доработку сотруднику — статус станет Draft.</summary>
        Task<ManagerVacationRequestDto> ReturnForRevisionAsync(Guid requestId, Guid managerId, string? comment);

        /// <summary>Сотрудники подразделения руководителя.</summary>
        Task<IEnumerable<ManagerEmployeeDto>> GetEmployeesAsync(Guid managerId);
    }

    public record ManagerVacationRequestDto(
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

    public record ManagerEmployeeDto(
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        int? PositionId,
        string? PositionName);
}