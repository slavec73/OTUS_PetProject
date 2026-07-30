using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Enums;
using VacationPlanner.Models.Requests;
using VacationPlanner.Models.Responses;

namespace VacationPlanner.Api.Controllers;

[ApiController]
[Route("api/hr")]
[Authorize(Roles = WellKnownRoles.Hr)]
public class HrController : ControllerBase
{
    private readonly IHrService _hrService;

    public HrController(IHrService hrService)
    {
        _hrService = hrService;
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();
        return Guid.Parse(claim.Value);
    }

    /// <summary>
    /// Просмотр всех заявок на отпуск
    /// </summary>
    [HttpGet("vacation-requests")]
    public async Task<IActionResult> GetAllRequests()
    {
        var requests = await _hrService.GetAllRequestsAsync();
        var response = requests.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Просмотр заявок по позиции (отделу)
    /// </summary>
    [HttpGet("vacation-requests/by-position/{positionId:int}")]
    public async Task<IActionResult> GetRequestsByPosition(int positionId)
    {
        var requests = await _hrService.GetRequestsByPositionAsync(positionId);
        var response = requests.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Просмотр заявок по пользователю
    /// </summary>
    [HttpGet("vacation-requests/by-user/{userId:guid}")]
    public async Task<IActionResult> GetRequestsByUser(Guid userId)
    {
        var requests = await _hrService.GetRequestsByUserAsync(userId);
        var response = requests.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Просмотр заявок по статусу
    /// </summary>
    [HttpGet("vacation-requests/by-status/{status}")]
    public async Task<IActionResult> GetRequestsByStatus(VacationRequestStatus status)
    {
        var requests = await _hrService.GetRequestsByStatusAsync(status);
        var response = requests.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Согласование заявки на отпуск (2-й этап — HR)
    /// </summary>
    [HttpPost("vacation-requests/{id:guid}/approve")]
    public async Task<IActionResult> ApproveRequest(Guid id, [FromBody] HrApproveRequest request)
    {
        var hrUserId = GetCurrentUserId();
        var result = await _hrService.ApproveRequestAsync(id, hrUserId, request.Comment);
        return Ok(MapToResponse(result));
    }

    /// <summary>
    /// Возврат заявки на доработку
    /// </summary>
    [HttpPost("vacation-requests/{id:guid}/return")]
    public async Task<IActionResult> ReturnForRevision(Guid id, [FromBody] HrReturnForRevisionRequest request)
    {
        var hrUserId = GetCurrentUserId();
        var result = await _hrService.ReturnForRevisionAsync(id, hrUserId, request.Comment);
        return Ok(MapToResponse(result));
    }

    /// <summary>
    /// Оформление отпуска сотруднику по согласованной заявке
    /// </summary>
    [HttpPost("vacation-requests/{id:guid}/create-vacation")]
    public async Task<IActionResult> CreateVacation(Guid id)
    {
        var hrUserId = GetCurrentUserId();
        var result = await _hrService.CreateVacationFromRequestAsync(id, hrUserId);
        return Ok(result);
    }

    /// <summary>
    /// Просмотр всех отпусков
    /// </summary>
    [HttpGet("vacations")]
    public async Task<IActionResult> GetAllVacations()
    {
        var vacations = await _hrService.GetAllVacationsAsync();
        return Ok(vacations);
    }

    /// <summary>
    /// Просмотр отпусков за период
    /// </summary>
    [HttpGet("vacations/by-date-range")]
    public async Task<IActionResult> GetVacationsByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var vacations = await _hrService.GetVacationsByDateRangeAsync(from, to);
        return Ok(vacations);
    }

    private static HrVacationRequestResponse MapToResponse(HrVacationRequestDto dto)
    {
        return new HrVacationRequestResponse
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
