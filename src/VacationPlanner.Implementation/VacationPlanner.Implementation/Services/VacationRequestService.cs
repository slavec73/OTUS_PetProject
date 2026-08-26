using Microsoft.Extensions.Logging;
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
        private readonly IUserRepository _userRepository;
        private readonly ILogger<VacationRequestService> _logger;

        public VacationRequestService(
            IVacationRequestRepository requestRepository,
            IVacationApprovalRepository approvalRepository,
            IUserRepository userRepository,
            ILogger<VacationRequestService> logger)
        {
            _requestRepository = requestRepository;
            _approvalRepository = approvalRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<VacationRequestDto?> GetByIdAsync(Guid id, Guid currentUserId)
        {
            _logger.LogInformation("Start get rerquest by id");
            var request = await _requestRepository.GetByIdAsync(id);
            if (request == null)
            {
                _logger.LogWarning($"request by id: {id} not found");
                return null;
            }

            if (request.UserId != currentUserId)
            {
                _logger.LogWarning($"The current user is not the author of the request with id: {id}");
                return null;
            }

            _logger.LogInformation("End get rerquest by id");
            return MapToDto(request);
        }

        public async Task<IEnumerable<VacationRequestDto>> GetMyRequestsAsync(Guid userId)
        {
            _logger.LogInformation("Start get rerquests by userid");
            var requests = await _requestRepository.GetByUserIdAsync(userId);
            _logger.LogInformation("End get rerquests by userid");
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<VacationRequestDto>> GetPendingApprovalsAsync(Guid approverUserId)
        {
            _logger.LogInformation("Start get pending rerquests by userid");
            var requests = await _requestRepository.GetPendingApprovalsForApproverAsync(approverUserId);
            _logger.LogInformation("Start get pending rerquests by userid");
            return requests.Select(MapToDto);
        }

        public async Task<VacationRequestDto> CreateAsync(CreateVacationRequestDto dto, Guid userId)
        {
            _logger.LogInformation("Start create rerquest");
            var author = await _userRepository.FindUserByIdAsync(userId);
            if (author.DepartmentId is null)
            {
                throw new InvalidOperationException("Вы не приняты в подразделение. Обратитесь к администратору системы");
            }

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
            _logger.LogInformation("End creating request");

            return MapToDto(request);
        }

        public async Task<VacationRequestDto> UpdateDraftAsync(Guid id, UpdateVacationRequestDto dto, Guid userId)
        {
            _logger.LogInformation("Start update draft rerquest by id");
            var request = await _requestRepository.GetByIdAsync(id);

            if (request is null)
            {
                _logger.LogError($"request with id: {id} not found");
                throw new InvalidOperationException("Заявка не найдена");
            }

            if (request.UserId != userId)
            {
                _logger.LogError($"user with id: {userId} not have access to request with id :{id}");
                throw new UnauthorizedAccessException("Нет доступа к этой заявке");
            }

            if (request.Status != VacationRequestStatus.Draft)
            {
                _logger.LogError($"status request with id: {id} no equal Draft");
                throw new InvalidOperationException("Можно редактировать только заявки в статусе \"Черновик\"");
            }

            request.Reason = dto.Reason;
            request.DateFrom = dto.DateFrom;
            request.DateTo = dto.DateTo;
            request.Comment = dto.Comment;
            request.UpdatedAt = DateTime.UtcNow;

            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            _logger.LogInformation("End update draft rerquest by id");
            return MapToDto(request);
        }

        public async Task<VacationRequestDto> SubmitForApprovalAsync(Guid id, Guid userId)
        {
            _logger.LogInformation($"Start submit request");
            var request = await _requestRepository.GetByIdAsync(id);
            if (request is null)
            {
                _logger.LogError($"status request with id: {id} not fount");
                throw new InvalidOperationException("Заявка не найдена");
            }


            if (request.UserId != userId)
            {
                _logger.LogError($"user with id: {userId} not have access to request with id :{id}");
                throw new UnauthorizedAccessException("Нет доступа к этой заявке");
            }

            if (request.Status != VacationRequestStatus.Draft)
            {
                _logger.LogError($"status request with id: {id} no equal Draft");
                throw new InvalidOperationException("Можно редактировать только заявки в статусе \"Черновик\"");
            }

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
            _logger.LogInformation($"End submit request");
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
