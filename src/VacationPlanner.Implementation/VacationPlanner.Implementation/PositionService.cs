using VacationPlanner.Interfaces;
using VacationPlanner.Models;

namespace VacationPlanner.Implementation.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _repository;
        public PositionService(IPositionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PositionDto>> GetAllPositionsAsync()
        {
            var positions = await _repository.GetAllAsync();
            return positions.Select(p => new PositionDto(p.Id, p.Name, p.Description));
        }

        public async Task<PositionDto> GetPositionByIdAsync(int id)
        {
            var pos = await _repository.GetByIdAsync(id);
            if (pos == null) return null;
            return new PositionDto(pos.Id, pos.Name, pos.Description);
        }

        public async Task<PositionDto> CreatePositionAsync(CreatePositionDto dto)
        {
            var position = new Position
            {
                Name = dto.Name,
                Description = dto.Description
            };
            await _repository.AddAsync(position);
            await _repository.SaveChangesAsync();
            return new PositionDto(position.Id, position.Name, position.Description);
        }

        public async Task<PositionDto> UpdatePositionAsync(int id, UpdatePositionDto dto)
        {
            var position = await _repository.GetByIdAsync(id);
            if (position == null) return null;
            position.Name = dto.Name;
            position.Description = dto.Description;
            _repository.Update(position);
            await _repository.SaveChangesAsync();
            return new PositionDto(position.Id, position.Name, position.Description);
        }

        public async Task<bool> DeletePositionAsync(int id)
        {
            var position = await _repository.GetByIdAsync(id);
            if (position == null) return false;
            _repository.Delete(position);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
