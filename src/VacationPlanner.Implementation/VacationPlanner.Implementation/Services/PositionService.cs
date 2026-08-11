using Microsoft.Extensions.Logging;
using VacationPlanner.Interfaces.Repository;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Implementation.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _repository;
        private readonly ILogger<PositionService> _logger;
        public PositionService(IPositionRepository repository, ILogger<PositionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<PositionDto>> GetAllPositionsAsync()
        {
            _logger.LogInformation($"Start Get All Positions");
            var positions = await _repository.GetAllAsync();
            _logger.LogInformation($"End Get All Positions");
            return positions.Select(p => new PositionDto(p.Id, p.Name, p.Description));
        }

        public async Task<PositionDto?> GetPositionByIdAsync(int id)
        {
            _logger.LogInformation($"Start Get Position By Id");
            var pos = await _repository.GetByIdAsync(id);
            _logger.LogInformation($"End Get Position By Id");
            if (pos == null) return null;
            return new PositionDto(pos.Id, pos.Name, pos.Description);
        }

        public async Task<PositionDto> CreatePositionAsync(CreatePositionDto dto)
        {
            _logger.LogInformation($"Start Creating Position");
            var position = new Position
            {
                Name = dto.Name,
                Description = dto.Description
            };
            await _repository.AddAsync(position);
            await _repository.SaveChangesAsync();
            _logger.LogInformation($"Position succefull created");
            return new PositionDto(position.Id, position.Name, position.Description);
        }

        public async Task<PositionDto?> UpdatePositionAsync(int id, UpdatePositionDto dto)
        {
            _logger.LogInformation($"Start Updating Position");
            var position = await _repository.GetByIdAsync(id);
            if (position == null) return null;
            position.Name = dto.Name;
            position.Description = dto.Description;
            _repository.Update(position);
            await _repository.SaveChangesAsync();
            _logger.LogInformation($"Position succefull updated");
            return new PositionDto(position.Id, position.Name, position.Description);
        }

        public async Task<bool> DeletePositionAsync(int id)
        {
            _logger.LogInformation($"Start Deleting Position");
            var position = await _repository.GetByIdAsync(id);
            if (position == null)
            {
                _logger.LogWarning($"Position with id: {id} not found");
                return false;
            }

            _repository.Delete(position);
            await _repository.SaveChangesAsync();
            _logger.LogInformation($"Position succefull deleted");
            return true;
        }
    }
}
