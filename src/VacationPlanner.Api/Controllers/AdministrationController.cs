using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VacationPlanner.Interfaces.Services;
using VacationPlanner.Models.DbModels;

namespace VacationPlanner.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = WellKnownRoles.Administrator)]
    public class AdministrationController : ControllerBase
    {
        private readonly IPositionService _positionService;
        private readonly IVacationDurationService _vacationDurationService;
        private readonly IUserRoleService _userRoleService;

        public AdministrationController(
            IPositionService positionService,
            IVacationDurationService vacationDurationService,
            IUserRoleService userRoleService)
        {
            _positionService = positionService;
            _vacationDurationService = vacationDurationService;
            _userRoleService = userRoleService;
        }

        [HttpGet("positions")]
        public async Task<IActionResult> GetAllPositions()
        {
            var positions = await _positionService.GetAllPositionsAsync();
            return Ok(positions);
        }

        [HttpGet("positions/{id}")]
        public async Task<IActionResult> GetPositionById(int id)
        {
            var position = await _positionService.GetPositionByIdAsync(id);
            if (position == null)
                return NotFound();

            return Ok(position);
        }

        [HttpPost("positions")]
        public async Task<IActionResult> CreatePosition([FromBody] CreatePositionDto dto)
        {
            var created = await _positionService.CreatePositionAsync(dto);
            return CreatedAtAction(nameof(GetPositionById), new { id = created.Id }, created);
        }

        [HttpPut("positions/{id}")]
        public async Task<IActionResult> UpdatePosition(int id, [FromBody] UpdatePositionDto dto)
        {
            var updated = await _positionService.UpdatePositionAsync(id, dto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        [HttpDelete("positions/{id}")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var deleted = await _positionService.DeletePositionAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpPost("vacation/global")]
        public async Task<IActionResult> SetGlobalVacationDays([FromBody] int days)
        {
            await _vacationDurationService.SetGlobalVacationDurationAsync(days);
            return Ok();
        }

        [HttpPost("vacation/by-position/{positionId}")]
        public async Task<IActionResult> SetVacationDaysForPosition(int positionId, [FromBody] int days)
        {
            try
            {
                await _vacationDurationService.SetVacationDurationByPositionAsync(positionId, days);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRoleService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userRoleService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("users/{userId}/role")]
        public async Task<IActionResult> ChangeUserRole(Guid userId, [FromBody] Guid newRole)
        {
            var response = await _userRoleService.ChangeUserRoleAsync(userId, newRole);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok();
        }

        [HttpPost("users/{userId}/department")]
        public async Task<IActionResult> ChangeUserDepartment(Guid userId, [FromBody] int departmentId)
        {
            var response = await _userRoleService.ChangeUserDepartmentAsync(userId, departmentId);
            if (!response.Success)
                return BadRequest(response.Message);
            return Ok();
        }
    }
}
