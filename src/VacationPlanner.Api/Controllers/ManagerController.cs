using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Enums;
using VacationPlanner.Models.Requests;
using VacationPlanner.Models.Responses;

namespace VacationPlanner.Api.Controllers;

/// <summary>
/// Функциональность руководителя (менеджера) — урезанная версия HR:
/// руководитель видит и согласовывает заявки сотрудников только своего подразделения.
/// </summary>
[ApiController]
[Route("api/manager")]
[Authorize(Roles = WellKnownRoles.Manager)]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;

    public ManagerController(IManagerService managerService)
    {
        _managerService = managerService;
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();
        return Guid.Parse(claim.Value);
    }

    /// <summary>
    /// Заявки сотрудников своего подразделения, ожидающие согласования руководителя (1-й этап)
    /// </summary>
    [HttpGet("vacation-requests/pending")]
    public async Task<IActionResult> GetPendingRequests()
    {
        var managerId = GetCurrentUserId();
        var requests = await _managerService.GetPendingRequestsAsync(managerId);
        return Ok(requests.Select(MapToResponse));
    }

    /// <summary>
    /// Заявки сотрудника своего подразделения
    /// </summary>
    [HttpGet("vacation-requests/by-employee/{employeeId:guid}")]
    public async Task<IActionResult> GetRequestsByEmployee(Guid employeeId)
    {
        var managerId = GetCurrentUserId();
        var requests = await _managerService.GetRequestsByEmployeeAsync(managerId, employeeId);
        return Ok(requests.Select(MapToResponse));
    }

    /// <summary>
    /// Заявки сотрудников подразделения по статусу
    /// </summary>
    [HttpGet("vacation-requests/by-status/{status}")]
    public async Task<IActionResult> GetRequestsByStatus(VacationRequestStatus status)
    {
        var managerId = GetCurrentUserId();
        var requests = await _managerService.GetRequestsByStatusAsync(managerId, status);
        return Ok(requests.Select(MapToResponse));
    }

    /// <summary>
    /// Конкретная заявка сотрудника своего подразделения
    /// </summary>
    [HttpGet("vacation-requests/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var managerId = GetCurrentUserId();
        var request = await _managerService.GetByIdAsync(id, managerId);
        if (request is null)
            return NotFound();

        return Ok(MapToResponse(request));
    }

    /// <summary>
    /// Согласовать заявку (1-й этап) — заявка уходит в HR (статус PendingSecondApproval)
    /// </summary>
    [HttpPost("vacation-requests/{id:guid}/approve")]
    public async Task<IActionResult> ApproveRequest(Guid id, [FromBody] ManagerApproveRequest request)
    {
        var managerId = GetCurrentUserId();
        var result = await _managerService.ApproveRequestAsync(id, managerId, request.Comment);
        return Ok(MapToResponse(result));
    }

    /// <summary>
    /// Вернуть заявку на доработку сотруднику (статус Draft)
    /// </summary>
    [HttpPost("vacation-requests/{id:guid}/return")]
    public async Task<IActionResult> ReturnForRevision(Guid id, [FromBody] ManagerReturnForRevisionRequest request)
    {
        var managerId = GetCurrentUserId();
        var result = await _managerService.ReturnForRevisionAsync(id, managerId, request.Comment);
        return Ok(MapToResponse(result));
    }

    /// <summary>
    /// Сотрудники своего подразделения
    /// </summary>
    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees()
    {
        var managerId = GetCurrentUserId();
        var employees = await _managerService.GetEmployeesAsync(managerId);
        return Ok(employees);
    }

    private static ManagerVacationRequestResponse MapToResponse(ManagerVacationRequestDto dto)
    {
        return new ManagerVacationRequestResponse
        {
            Id = dto.Id,
            UserId = dto.UserId,
            UserEmail = dto.UserEmail,
            UserName = dto.UserName,
            PositionId = dto.PositionId,
            PositionName = dto.PositionName,
            Reason = dto.Reason,
            DateFrom = dto.DateFrom,
            DateTo = dto.DateTo,
            TotalDays = dto.TotalDays,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            Comment = dto.Comment
        };
    }
}
