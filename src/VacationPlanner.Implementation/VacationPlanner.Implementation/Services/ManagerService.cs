using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Enums;

namespace VacationPlanner.Implementation.Services
{

    public class ManagerService : IManagerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IVacationRequestRepository _requestRepository;
        private readonly IVacationApprovalRepository _approvalRepository;
        private readonly ILogger<ManagerService> _logger;

        public ManagerService(
            ApplicationDbContext context,
            IVacationRequestRepository requestRepository,
            IVacationApprovalRepository approvalRepository,
            ILogger<ManagerService> logger)
        {
            _context = context;
            _requestRepository = requestRepository;
            _approvalRepository = approvalRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ManagerVacationRequestDto>> GetPendingRequestsAsync(Guid managerId)
        {
            _logger.LogInformation($"Start get pending requests by manager: {managerId}");
            var department = await GetDepartmentAsync(managerId);
            if (department is null)
                return Enumerable.Empty<ManagerVacationRequestDto>();

            var requests = await _context.VacationRequests
                .Include(vr => vr.User)
                .ThenInclude(u => u!.Position)
                .Where(vr => vr.User != null
                             && vr.User.DepartmentId == department.DepartmentId
                             && vr.Status == VacationRequestStatus.PendingFirstApproval)
                .OrderByDescending(vr => vr.CreatedAt)
                .ToListAsync();

            _logger.LogInformation($"End get pending requests by manager: {managerId}");
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<ManagerVacationRequestDto>> GetRequestsByEmployeeAsync(Guid managerId, Guid employeeId)
        {
            _logger.LogInformation($"Start get requests by employee: {employeeId} for manager: {managerId}");
            var department = await GetDepartmentAsync(managerId);
            if (department is null || !await IsSubordinateAsync(department.DepartmentId, employeeId))
                return Enumerable.Empty<ManagerVacationRequestDto>();

            var requests = await _context.VacationRequests
                .Include(vr => vr.User)
                .ThenInclude(u => u!.Position)
                .Where(vr => vr.UserId == employeeId)
                .OrderByDescending(vr => vr.CreatedAt)
                .ToListAsync();

            _logger.LogInformation($"End get requests by employee: {employeeId} for manager: {managerId}");
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<ManagerVacationRequestDto>> GetRequestsByStatusAsync(Guid managerId, VacationRequestStatus status)
        {
            _logger.LogInformation($"Start get requests by status: {status} for manager: {managerId}");
            var department = await GetDepartmentAsync(managerId);
            if (department is null)
                return Enumerable.Empty<ManagerVacationRequestDto>();

            var requests = await _context.VacationRequests
                .Include(vr => vr.User)
                .ThenInclude(u => u!.Position)
                .Where(vr => vr.User != null
                             && vr.User.DepartmentId == department.DepartmentId
                             && vr.Status == status)
                .OrderByDescending(vr => vr.CreatedAt)
                .ToListAsync();

            _logger.LogInformation($"End get requests by status: {status} for manager: {managerId}");
            return requests.Select(MapToDto);
        }

        public async Task<ManagerVacationRequestDto?> GetByIdAsync(Guid requestId, Guid managerId)
        {
            _logger.LogInformation($"Start get request by id: {requestId} for manager: {managerId}");
            var department = await GetDepartmentAsync(managerId);
            if (department is null)
                return null;

            var request = await _context.VacationRequests
                .Include(vr => vr.User)
                .ThenInclude(u => u!.Position)
                .FirstOrDefaultAsync(vr => vr.VacationRequestId == requestId
                                           && vr.User != null
                                           && vr.User.DepartmentId == department.DepartmentId);

            _logger.LogInformation($"End get request by id: {requestId} for manager: {managerId}");
            return request is null ? null : MapToDto(request);
        }

        public async Task<ManagerVacationRequestDto> ApproveRequestAsync(Guid requestId, Guid managerId, string? comment)
        {
            _logger.LogInformation($"Start approve request: {requestId} by manager: {managerId}");
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request is null)
            {
                _logger.LogError($"request with id: {requestId} not found");
                throw new InvalidOperationException("Заявка не найдена");
            }

            await EnsureDepartmentAccessAsync(request, managerId);

            if (request.Status != VacationRequestStatus.PendingFirstApproval)
            {
                _logger.LogError($"status request with id: {requestId} not equal PendingFirstApproval");
                throw new InvalidOperationException("Можно согласовать только заявки, ожидающие согласования руководителя");
            }

            request.Status = VacationRequestStatus.PendingSecondApproval;
            request.Comment = comment;
            request.UpdatedAt = DateTime.UtcNow;

            _requestRepository.Update(request);

            var approval = new VacationApproval
            {
                VacationRequestId = requestId,
                ApprovalStage = 1,
                ApproverUserId = managerId,
                Decision = VacationRequestStatus.Approved,
                Comment = comment
            };
            await _approvalRepository.AddAsync(approval);

            await _requestRepository.SaveChangesAsync();

            _logger.LogInformation($"End approve request: {requestId} by manager: {managerId}");
            return await LoadDtoAsync(requestId);
        }

        public async Task<ManagerVacationRequestDto> ReturnForRevisionAsync(Guid requestId, Guid managerId, string? comment)
        {
            _logger.LogInformation($"Start return for revision request: {requestId} by manager: {managerId}");
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request is null)
            {
                _logger.LogError($"request with id: {requestId} not found");
                throw new InvalidOperationException("Заявка не найдена");
            }

            await EnsureDepartmentAccessAsync(request, managerId);

            if (request.Status != VacationRequestStatus.PendingFirstApproval)
            {
                _logger.LogError($"status request with id: {requestId} not equal PendingFirstApproval");
                throw new InvalidOperationException("Можно вернуть на доработку только заявки, ожидающие согласования руководителя");
            }

            request.Status = VacationRequestStatus.Draft;
            request.Comment = comment;
            request.UpdatedAt = DateTime.UtcNow;

            _requestRepository.Update(request);

            var approval = new VacationApproval
            {
                VacationRequestId = requestId,
                ApprovalStage = 1,
                ApproverUserId = managerId,
                Decision = VacationRequestStatus.Rejected,
                Comment = comment
            };
            await _approvalRepository.AddAsync(approval);

            await _requestRepository.SaveChangesAsync();

            _logger.LogInformation($"End return for revision request: {requestId} by manager: {managerId}");
            return await LoadDtoAsync(requestId);
        }

        public async Task<IEnumerable<ManagerEmployeeDto>> GetEmployeesAsync(Guid managerId)
        {
            _logger.LogInformation($"Start get employees for manager: {managerId}");
            var department = await GetDepartmentAsync(managerId);
            if (department is null)
                return Enumerable.Empty<ManagerEmployeeDto>();

            var employees = await _context.Users
                .Include(u => u.Position)
                .Where(u => u.DepartmentId == department.DepartmentId)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            _logger.LogInformation($"End get employees for manager: {managerId}");
            return employees.Select(u => new ManagerEmployeeDto(
                u.UserId,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PositionId,
                u.Position?.Name));
        }

        private Task<Department?> GetDepartmentAsync(Guid managerId)
        {
            return _context.Departments
                .FirstOrDefaultAsync(d => d.ManagerId == managerId);
        }

        private Task<bool> IsSubordinateAsync(int departmentId, Guid employeeId)
        {
            return _context.Users
                .AnyAsync(u => u.UserId == employeeId && u.DepartmentId == departmentId);
        }

        private async Task EnsureDepartmentAccessAsync(VacationRequest request, Guid managerId)
        {
            var department = await GetDepartmentAsync(managerId);
            if (department is null)
            {
                _logger.LogError($"manager: {managerId} not found as department manager");
                throw new UnauthorizedAccessException("Вы не являетесь руководителем подразделения");
            }

            if (request.User == null || request.User.DepartmentId != department.DepartmentId)
            {
                _logger.LogError($"manager: {managerId} tries to work with request: {request.VacationRequestId} of another department");
                throw new UnauthorizedAccessException("Заявка не относится к вашему подразделению");
            }
        }

        private async Task<ManagerVacationRequestDto> LoadDtoAsync(Guid requestId)
        {
            var request = await _context.VacationRequests
                .Include(vr => vr.User)
                .ThenInclude(u => u!.Position)
                .FirstOrDefaultAsync(vr => vr.VacationRequestId == requestId);
            return MapToDto(request!);
        }

        private static ManagerVacationRequestDto MapToDto(VacationRequest request)
        {
            return new ManagerVacationRequestDto(
                request.VacationRequestId,
                request.UserId,
                request.User?.Email,
                request.User != null ? $"{request.User.FirstName} {request.User.LastName}" : null,
                request.User?.PositionId,
                request.User?.Position?.Name,
                request.Reason,
                request.DateFrom,
                request.DateTo,
                request.TotalDays,
                request.Status.ToString(),
                request.CreatedAt,
                request.Comment);
        }
    }
}
