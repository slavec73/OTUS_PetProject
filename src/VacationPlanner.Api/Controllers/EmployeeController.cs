using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.Requests;
using VacationPlanner.Models.Responses;


namespace VacationPlanner.Api.Controllers;

[ApiController]
[Route("api/employee")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IVacationRequestService _vacationRequestService;
    private readonly IVacationService _vacationService;

    public EmployeeController(
        IVacationRequestService vacationRequestService,
        IVacationService vacationService)
    {
        _vacationRequestService = vacationRequestService;
        _vacationService = vacationService;
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();
        return Guid.Parse(claim.Value);
    }

    /// <summary>
    /// Просмотр своих заявок на согласование отпуска
    /// </summary>
    [HttpGet("vacation-requests/my")]
    public async Task<ActionResult<IEnumerable<VacationRequestResponse>>> GetMyRequests()
    {
        var userId = GetCurrentUserId();
        var requests = await _vacationRequestService.GetMyRequestsAsync(userId);
        var response = requests.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Просмотр заявок, ожидающих согласования текущим пользователем
    /// </summary>
    [HttpGet("vacation-requests/pending")]
    public async Task<ActionResult<IEnumerable<VacationRequestResponse>>> GetPendingApprovals()
    {
        var userId = GetCurrentUserId();
        var requests = await _vacationRequestService.GetPendingApprovalsAsync(userId);
        var response = requests.Select(MapToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Просмотр конкретной заявки
    /// </summary>
    [HttpGet("vacation-requests/{id:guid}")]
    public async Task<ActionResult<VacationRequestResponse>> GetById(Guid id)
    {
        var userId = GetCurrentUserId();
        var request = await _vacationRequestService.GetByIdAsync(id, userId);
        if (request == null)
            return NotFound();

        return Ok(MapToResponse(request));
    }

    /// <summary>
    /// Создание заявки на отпуск
    /// </summary>
    [HttpPost("vacation-requests")]
    public async Task<ActionResult<VacationRequestResponse>> Create(
        [FromBody] CreateVacationRequest request)
    {
        var userId = GetCurrentUserId();
        var dto = new CreateVacationRequestDto(
            request.Reason,
            request.DateFrom,
            request.DateTo,
            request.Comment);

        var created = await _vacationRequestService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponse(created));
    }

    /// <summary>
    /// Редактирование заявки в статусе "Черновик"
    /// </summary>
    [HttpPut("vacation-requests/{id:guid}")]
    public async Task<ActionResult<VacationRequestResponse>> UpdateDraft(
        Guid id,
        [FromBody] UpdateVacationRequest request)
    {
        var userId = GetCurrentUserId();
        var dto = new UpdateVacationRequestDto(
            request.Reason,
            request.DateFrom,
            request.DateTo,
            request.Comment);

        var updated = await _vacationRequestService.UpdateDraftAsync(id, dto, userId);
        return Ok(MapToResponse(updated));
    }

    /// <summary>
    /// Отправить заявку на согласование на 1-й этап
    /// </summary>
    [HttpPost("vacation-requests/{id:guid}/submit")]
    public async Task<ActionResult<VacationRequestResponse>> SubmitForApproval(Guid id)
    {
        var userId = GetCurrentUserId();
        var submitted = await _vacationRequestService.SubmitForApprovalAsync(id, userId);
        return Ok(MapToResponse(submitted));
    }

    /// <summary>
    /// Просмотр своих отпусков
    /// </summary>
    [HttpGet("vacations/my")]
    public async Task<ActionResult<IEnumerable<VacationResponse>>> GetMyVacations()
    {
        var userId = GetCurrentUserId();
        var vacations = await _vacationService.GetMyVacationsAsync(userId);
        var response = vacations.Select(v => new VacationResponse
        {
            Id = v.Id,
            DateFrom = v.DateFrom,
            DateTo = v.DateTo,
            TotalDays = v.TotalDays,
            VacationType = v.VacationType,
            IsPaid = v.IsPaid,
            CreatedAt = v.CreatedAt
        });
        return Ok(response);
    }

    private static VacationRequestResponse MapToResponse(VacationRequestDto dto)
    {
        return new VacationRequestResponse
        {
            Id = dto.Id,
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
