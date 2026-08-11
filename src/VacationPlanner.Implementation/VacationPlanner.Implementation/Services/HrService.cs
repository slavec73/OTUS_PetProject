using Microsoft.Extensions.Logging;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Enums;

namespace VacationPlanner.Implementation.Services
{
    public class HrService : IHrService
    {
        private readonly IVacationRequestRepository _requestRepository;
        private readonly IVacationApprovalRepository _approvalRepository;
        private readonly IVacationRepository _vacationRepository;
        private readonly ILogger<HrService> _logger;

        public HrService(
            IVacationRequestRepository requestRepository,
            IVacationApprovalRepository approvalRepository,
            IVacationRepository vacationRepository,
            ILogger<HrService> logger)
        {
            _requestRepository = requestRepository;
            _approvalRepository = approvalRepository;
            _vacationRepository = vacationRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetAllRequestsAsync()
        {
            _logger.LogInformation("Start get requests by HR");
            var requests = await _requestRepository.GetAllAsync();
            _logger.LogInformation("End get requests by HR");
            return requests.Select(MapToHrDto);
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetRequestsByPositionAsync(int positionId)
        {
            _logger.LogInformation("Start get requests by position by HR");
            var requests = await _requestRepository.GetByPositionIdAsync(positionId);
            _logger.LogInformation("Start get requests by position by HR");
            return requests.Select(MapToHrDto);
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetRequestsByUserAsync(Guid userId)
        {
            _logger.LogInformation($"Start get requests by userId: {userId} by HR");
            var requests = await _requestRepository.GetByUserIdAsync(userId);
            _logger.LogInformation($"End get requests by userId: {userId} by HR");
            return requests.Select(MapToHrDto);
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetRequestsByStatusAsync(VacationRequestStatus status)
        {
            _logger.LogInformation("Start get requests by status");
            var requests = await _requestRepository.GetByStatusAsync(status);
            _logger.LogInformation("End get requests by status");
            return requests.Select(MapToHrDto);
        }

        public async Task<HrVacationRequestDto> ApproveRequestAsync(Guid requestId, Guid hrUserId, string? comment)
        {
            _logger.LogInformation($"Start approve request: {requestId}");
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request is null)
            {
                _logger.LogError($"reqeust with id: {requestId} not found");
                throw new InvalidOperationException("Заявка не найдена");
            }

            if (request.Status != VacationRequestStatus.PendingFirstApproval)
            {
                _logger.LogError($"status reqeust with id: {requestId} not equal PendingFirstApproval");
                throw new InvalidOperationException("Можно согласовать только заявки в статусе \"На согласовании\"");
            }

            request.Status = VacationRequestStatus.Approved;
            request.Comment = comment;
            request.UpdatedAt = DateTime.UtcNow;

            _requestRepository.Update(request);

            var approval = new VacationApproval
            {
                VacationRequestId = requestId,
                ApprovalStage = 1,
                ApproverUserId = hrUserId,
                Decision = VacationRequestStatus.Approved,
                Comment = comment
            };
            await _approvalRepository.AddAsync(approval);

            await _requestRepository.SaveChangesAsync();

            _logger.LogInformation($"End approve request: {requestId}");
            return MapToHrDto(request);
        }

        public async Task<HrVacationRequestDto> ReturnForRevisionAsync(Guid requestId, Guid hrUserId, string? comment)
        {
            _logger.LogInformation($"Start return for revision request: {requestId}");
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request is null)
            {
                _logger.LogError($"reqeust with id: {requestId} not found");
                throw new InvalidOperationException("Заявка не найдена");
            }

            if (request.Status != VacationRequestStatus.PendingFirstApproval)
            {
                _logger.LogError($"status reqeust with id: {requestId} not equal PendingFirstApproval");
                throw new InvalidOperationException("Можно вернуть на доработку только заявки в статусе \"На согласовании\"");
            }

            request.Status = VacationRequestStatus.Draft;
            request.Comment = comment;
            request.UpdatedAt = DateTime.UtcNow;

            _requestRepository.Update(request);

            var approval = new VacationApproval
            {
                VacationRequestId = requestId,
                ApprovalStage = 1,
                ApproverUserId = hrUserId,
                Decision = VacationRequestStatus.Rejected,
                Comment = comment
            };
            await _approvalRepository.AddAsync(approval);

            await _requestRepository.SaveChangesAsync();

            _logger.LogInformation($"End return for revision request: {requestId}");
            return MapToHrDto(request);
        }

        public async Task<VacationDto> CreateVacationFromRequestAsync(Guid requestId, Guid hrUserId)
        {
            _logger.LogInformation($"start create vacation");
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request is null)
            {
                _logger.LogError($"reqeust with id: {requestId} not found");
                throw new InvalidOperationException("Заявка не найдена");
            }

            if (request.Status != VacationRequestStatus.Approved)
            {
                _logger.LogError($"status reqeust with id: {requestId} not equal Approved");
                throw new InvalidOperationException("Можно оформить отпуск только по согласованной заявке");
            }

            var vacation = new Vacation
            {
                UserId = request.UserId,
                VacationRequestId = request.VacationRequestId,
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                VacationType = "Annual",
                IsPaid = true,
                CreatedAt = DateTime.UtcNow
            };

            await _vacationRepository.AddAsync(vacation);
            await _vacationRepository.SaveChangesAsync();
            _logger.LogInformation($"end create vacation");
            return new VacationDto(
                vacation.VacationId,
                vacation.DateFrom,
                vacation.DateTo,
                vacation.TotalDays,
                vacation.VacationType,
                vacation.IsPaid,
                vacation.CreatedAt);
        }

        public async Task<IEnumerable<VacationDto>> GetAllVacationsAsync()
        {
            _logger.LogInformation("Start get all vacations");
            var vacations = await _vacationRepository.GetAllAsync();
            _logger.LogInformation("End get all vacations");
            return vacations.Select(v => new VacationDto(
                v.VacationId,
                v.DateFrom,
                v.DateTo,
                v.TotalDays,
                v.VacationType,
                v.IsPaid,
                v.CreatedAt));
        }

        public async Task<IEnumerable<VacationDto>> GetVacationsByDateRangeAsync(DateTime from, DateTime to)
        {
            _logger.LogInformation("Start get all vacations by date range");
            var vacations = await _vacationRepository.GetByDateRangeAsync(from, to);
            _logger.LogInformation("Start get all vacations by date range");
            return vacations.Select(v => new VacationDto(
                v.VacationId,
                v.DateFrom,
                v.DateTo,
                v.TotalDays,
                v.VacationType,
                v.IsPaid,
                v.CreatedAt));
        }

        private static HrVacationRequestDto MapToHrDto(VacationRequest request)
        {
            return new HrVacationRequestDto(
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
