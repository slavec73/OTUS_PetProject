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

        public HrService(
            IVacationRequestRepository requestRepository,
            IVacationApprovalRepository approvalRepository,
            IVacationRepository vacationRepository)
        {
            _requestRepository = requestRepository;
            _approvalRepository = approvalRepository;
            _vacationRepository = vacationRepository;
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _requestRepository.GetAllAsync();
            return requests.Select(MapToHrDto);
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetRequestsByPositionAsync(int positionId)
        {
            var requests = await _requestRepository.GetByPositionIdAsync(positionId);
            return requests.Select(MapToHrDto);
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetRequestsByUserAsync(Guid userId)
        {
            var requests = await _requestRepository.GetByUserIdAsync(userId);
            return requests.Select(MapToHrDto);
        }

        public async Task<IEnumerable<HrVacationRequestDto>> GetRequestsByStatusAsync(VacationRequestStatus status)
        {
            var requests = await _requestRepository.GetByStatusAsync(status);
            return requests.Select(MapToHrDto);
        }

        public async Task<HrVacationRequestDto> ApproveRequestAsync(Guid requestId, Guid hrUserId, string? comment)
        {
            var request = await _requestRepository.GetByIdAsync(requestId)
                ?? throw new InvalidOperationException("Заявка не найдена");

            if (request.Status != VacationRequestStatus.PendingFirstApproval)
                throw new InvalidOperationException("Можно согласовать только заявки в статусе \"На согласовании\"");

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

            return MapToHrDto(request);
        }

        public async Task<HrVacationRequestDto> ReturnForRevisionAsync(Guid requestId, Guid hrUserId, string? comment)
        {
            var request = await _requestRepository.GetByIdAsync(requestId)
                ?? throw new InvalidOperationException("Заявка не найдена");

            if (request.Status != VacationRequestStatus.PendingFirstApproval)
                throw new InvalidOperationException("Можно вернуть на доработку только заявки в статусе \"На согласовании\"");

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

            return MapToHrDto(request);
        }

        public async Task<VacationDto> CreateVacationFromRequestAsync(Guid requestId, Guid hrUserId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId)
                ?? throw new InvalidOperationException("Заявка не найдена");

            if (request.Status != VacationRequestStatus.Approved)
                throw new InvalidOperationException("Можно оформить отпуск только по согласованной заявке");

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
            var vacations = await _vacationRepository.GetAllAsync();
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
            var vacations = await _vacationRepository.GetByDateRangeAsync(from, to);
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
