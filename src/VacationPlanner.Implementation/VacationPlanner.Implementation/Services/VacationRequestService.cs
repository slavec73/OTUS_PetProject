using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;
using VacationPlanner.Models.Enums;

namespace VacationPlanner.Implementation.Services
{
    public class VacationRequestService : IVacationRequestService
    {
        private readonly IVacationRequestRepository _requestRepository;
        private readonly IVacationApprovalRepository _approvalRepository;

        public VacationRequestService(
            IVacationRequestRepository requestRepository,
            IVacationApprovalRepository approvalRepository)
        {
            _requestRepository = requestRepository;
            _approvalRepository = approvalRepository;
        }

        public async Task<VacationRequestDto?> GetByIdAsync(Guid id, Guid currentUserId)
        {
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
                return null;

            if (request.UserId != currentUserId)
                return null;

            return MapToDto(request);
        }

        public async Task<IEnumerable<VacationRequestDto>> GetMyRequestsAsync(Guid userId)
        {
            var requests = await _requestRepository.GetByUserIdAsync(userId);
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<VacationRequestDto>> GetPendingApprovalsAsync(Guid approverUserId)
        {
            var requests = await _requestRepository.GetPendingApprovalsForApproverAsync(approverUserId);
            return requests.Select(MapToDto);
        }

        public async Task<VacationRequestDto> CreateAsync(CreateVacationRequestDto dto, Guid userId)
        {
            var request = new VacationRequest
            {
                UserId = userId,
                Reason = dto.Reason,
                DateFrom = dto.DateFrom,
                DateTo = dto.DateTo,
                Comment = dto.Comment,
                Status = VacationRequestStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            await _requestRepository.AddAsync(request);
            await _requestRepository.SaveChangesAsync();

            return MapToDto(request);
        }

        public async Task<VacationRequestDto> UpdateDraftAsync(Guid id, UpdateVacationRequestDto dto, Guid userId)
        {
            var request = await _requestRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Заявка не найдена");

            if (request.UserId != userId)
                throw new UnauthorizedAccessException("Нет доступа к этой заявке");

            if (request.Status != VacationRequestStatus.Draft)
                throw new InvalidOperationException("Можно редактировать только заявки в статусе \"Черновик\"");

            request.Reason = dto.Reason;
            request.DateFrom = dto.DateFrom;
            request.DateTo = dto.DateTo;
            request.Comment = dto.Comment;
            request.UpdatedAt = DateTime.UtcNow;

            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            return MapToDto(request);
        }

        public async Task<VacationRequestDto> SubmitForApprovalAsync(Guid id, Guid userId)
        {
            var request = await _requestRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Заявка не найдена");

            if (request.UserId != userId)
                throw new UnauthorizedAccessException("Нет доступа к этой заявке");

            if (request.Status != VacationRequestStatus.Draft)
                throw new InvalidOperationException("Можно отправить на согласование только заявки в статусе \"Черновик\"");

            request.Status = VacationRequestStatus.PendingFirstApproval;
            request.UpdatedAt = DateTime.UtcNow;

            _requestRepository.Update(request);

            var approval = new VacationApproval
            {
                VacationRequestId = id,
                ApprovalStage = 1,
                ApproverUserId = userId,
                Decision = VacationRequestStatus.PendingFirstApproval
            };
            await _approvalRepository.AddAsync(approval);

            await _requestRepository.SaveChangesAsync();

            return MapToDto(request);
        }

        private static VacationRequestDto MapToDto(VacationRequest request)
        {
            return new VacationRequestDto(
                request.VacationRequestId,
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
