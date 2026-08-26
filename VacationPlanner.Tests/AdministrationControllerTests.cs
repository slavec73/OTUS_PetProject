using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VacationPlanner.Api.Controllers;
using VacationPlanner.Interfaces.Services;
using Xunit;

namespace VacationPlanner.Tests
{
    public class AdministrationControllerTests
    {
        private readonly Mock<IPositionService> _positionServiceMock;
        private readonly Mock<IVacationDurationService> _vacationDurationServiceMock;
        private readonly Mock<IUserRoleService> _userRoleServiceMock;
        private readonly AdministrationController _controller;

        public AdministrationControllerTests()
        {
            _positionServiceMock = new Mock<IPositionService>();
            _vacationDurationServiceMock = new Mock<IVacationDurationService>();
            _userRoleServiceMock = new Mock<IUserRoleService>();

            _controller = new AdministrationController(
                _positionServiceMock.Object,
                _vacationDurationServiceMock.Object,
                _userRoleServiceMock.Object
            );
        }

        // ==================== GetAllPositions ====================

        /// <summary>
        /// Проверяет, что при наличии должностей метод возвращает 200 OK и список всех должностей.
        /// </summary>
        [Fact]
        public async Task GetAllPositions_WhenPositionsExist_ReturnsOkWithList()
        {
            // Arrange
            var expectedPositions = new List<PositionDto>
            {
                new PositionDto(1, "Developer", "Develops software"),
                new PositionDto(2, "Tester", "Tests software")
            };

            _positionServiceMock
                .Setup(x => x.GetAllPositionsAsync())
                .ReturnsAsync(expectedPositions);

            // Act
            var result = await _controller.GetAllPositions();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedPositions);
            _positionServiceMock.Verify(x => x.GetAllPositionsAsync(), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при отсутствии должностей метод возвращает 200 OK и пустой список.
        /// </summary>
        [Fact]
        public async Task GetAllPositions_WhenNoPositions_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyList = new List<PositionDto>();

            _positionServiceMock
                .Setup(x => x.GetAllPositionsAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAllPositions();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(emptyList);
            _positionServiceMock.Verify(x => x.GetAllPositionsAsync(), Times.Once);
        }

        // ==================== GetPositionById ====================

        /// <summary>
        /// Проверяет, что при существующем ID должности возвращается 200 OK и сама должность.
        /// </summary>
        [Fact]
        public async Task GetPositionById_WhenPositionExists_ReturnsOkWithPosition()
        {
            // Arrange
            const int positionId = 1;
            var expectedPosition = new PositionDto(positionId, "Developer", "Develops software");

            _positionServiceMock
                .Setup(x => x.GetPositionByIdAsync(positionId))
                .ReturnsAsync(expectedPosition);

            // Act
            var result = await _controller.GetPositionById(positionId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedPosition);
            _positionServiceMock.Verify(x => x.GetPositionByIdAsync(positionId), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при несуществующем ID возвращается 404 NotFound.
        /// </summary>
        [Fact]
        public async Task GetPositionById_WhenPositionNotFound_ReturnsNotFound()
        {
            // Arrange
            const int positionId = 999;

            _positionServiceMock
                .Setup(x => x.GetPositionByIdAsync(positionId))
                .ReturnsAsync((PositionDto?)null);

            // Act
            var result = await _controller.GetPositionById(positionId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _positionServiceMock.Verify(x => x.GetPositionByIdAsync(positionId), Times.Once);
        }

        // ==================== CreatePosition ====================

        /// <summary>
        /// Проверяет, что при корректных данных создаётся должность и возвращается 201 Created с ссылкой на получение.
        /// </summary>
        [Fact]
        public async Task CreatePosition_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreatePositionDto("Manager", "Manages team");
            var createdPosition = new PositionDto(3, "Manager", "Manages team");

            _positionServiceMock
                .Setup(x => x.CreatePositionAsync(It.IsAny<CreatePositionDto>()))
                .ReturnsAsync(createdPosition);

            // Act
            var result = await _controller.CreatePosition(createDto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(AdministrationController.GetPositionById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdPosition.Id);
            createdResult.Value.Should().BeEquivalentTo(createdPosition);

            _positionServiceMock.Verify(
                x => x.CreatePositionAsync(It.Is<CreatePositionDto>(dto =>
                    dto.Name == createDto.Name && dto.Description == createDto.Description)),
                Times.Once);
        }

        // ==================== UpdatePosition ====================

        /// <summary>
        /// Проверяет, что при обновлении существующей должности возвращается 200 OK с обновлёнными данными.
        /// </summary>
        [Fact]
        public async Task UpdatePosition_WhenPositionExists_ReturnsOkWithUpdatedPosition()
        {
            // Arrange
            const int positionId = 1;
            var updateDto = new UpdatePositionDto("Senior Developer", "Develops complex software");
            var updatedPosition = new PositionDto(positionId, "Senior Developer", "Develops complex software");

            _positionServiceMock
                .Setup(x => x.UpdatePositionAsync(positionId, updateDto))
                .ReturnsAsync(updatedPosition);

            // Act
            var result = await _controller.UpdatePosition(positionId, updateDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(updatedPosition);
            _positionServiceMock.Verify(x => x.UpdatePositionAsync(positionId, updateDto), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при обновлении несуществующей должности возвращается 404 NotFound.
        /// </summary>
        [Fact]
        public async Task UpdatePosition_WhenPositionNotFound_ReturnsNotFound()
        {
            // Arrange
            const int positionId = 999;
            var updateDto = new UpdatePositionDto("Test", "Test");

            _positionServiceMock
                .Setup(x => x.UpdatePositionAsync(positionId, updateDto))
                .ReturnsAsync((PositionDto?)null);

            // Act
            var result = await _controller.UpdatePosition(positionId, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _positionServiceMock.Verify(x => x.UpdatePositionAsync(positionId, updateDto), Times.Once);
        }

        // ==================== DeletePosition ====================

        /// <summary>
        /// Проверяет, что при успешном удалении должности возвращается 204 No Content.
        /// </summary>
        [Fact]
        public async Task DeletePosition_WhenPositionExists_ReturnsNoContent()
        {
            // Arrange
            const int positionId = 1;

            _positionServiceMock
                .Setup(x => x.DeletePositionAsync(positionId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePosition(positionId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _positionServiceMock.Verify(x => x.DeletePositionAsync(positionId), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при попытке удалить несуществующую должность возвращается 404 NotFound.
        /// </summary>
        [Fact]
        public async Task DeletePosition_WhenPositionNotFound_ReturnsNotFound()
        {
            // Arrange
            const int positionId = 999;

            _positionServiceMock
                .Setup(x => x.DeletePositionAsync(positionId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePosition(positionId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _positionServiceMock.Verify(x => x.DeletePositionAsync(positionId), Times.Once);
        }

        // ==================== SetGlobalVacationDays ====================

        /// <summary>
        /// Проверяет, что установка глобального количества дней отпуска с корректным значением возвращает 200 OK.
        /// </summary>
        [Fact]
        public async Task SetGlobalVacationDays_WithValidDays_ReturnsOk()
        {
            // Arrange
            const int days = 20;

            _vacationDurationServiceMock
                .Setup(x => x.SetGlobalVacationDurationAsync(days))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetGlobalVacationDays(days);

            // Assert
            result.Should().BeOfType<OkResult>();
            _vacationDurationServiceMock.Verify(x => x.SetGlobalVacationDurationAsync(days), Times.Once);
        }

        // ==================== SetVacationDaysForPosition ====================

        /// <summary>
        /// Проверяет, что установка дней отпуска для существующей должности возвращает 200 OK.
        /// </summary>
        [Fact]
        public async Task SetVacationDaysForPosition_WhenPositionExists_ReturnsOk()
        {
            // Arrange
            const int positionId = 1;
            const int days = 25;

            _vacationDurationServiceMock
                .Setup(x => x.SetVacationDurationByPositionAsync(positionId, days))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SetVacationDaysForPosition(positionId, days);

            // Assert
            result.Should().BeOfType<OkResult>();
            _vacationDurationServiceMock.Verify(x => x.SetVacationDurationByPositionAsync(positionId, days), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при попытке установить дни для несуществующей должности сервис выбрасывает ArgumentException,
        /// и контроллер преобразует его в 400 BadRequest с сообщением об ошибке.
        /// </summary>
        [Fact]
        public async Task SetVacationDaysForPosition_WhenPositionNotFound_ReturnsBadRequestWithErrorMessage()
        {
            // Arrange
            const int positionId = 999;
            const int days = 25;
            const string errorMessage = "Position not found";

            _vacationDurationServiceMock
                .Setup(x => x.SetVacationDurationByPositionAsync(positionId, days))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var result = await _controller.SetVacationDaysForPosition(positionId, days);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be(errorMessage);
            _vacationDurationServiceMock.Verify(x => x.SetVacationDurationByPositionAsync(positionId, days), Times.Once);
        }

        // ==================== GetAllUsers ====================

        /// <summary>
        /// Проверяет, что при наличии пользователей возвращается 200 OK и список всех пользователей.
        /// </summary>
        [Fact]
        public async Task GetAllUsers_WhenUsersExist_ReturnsOkWithList()
        {
            // Arrange
            var expectedUsers = new List<UserDto>
            {
                new UserDto
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "Alice",
                    Email = "alice@example.com",
                    Roles = new[] { "Admin" }
                },
                new UserDto
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "Bob",
                    Email = "bob@example.com",
                    Roles = new[] { "User" }
                }
            };

            _userRoleServiceMock
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedUsers);
            _userRoleServiceMock.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при отсутствии пользователей возвращается 200 OK и пустой список.
        /// </summary>
        [Fact]
        public async Task GetAllUsers_WhenNoUsersExist_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyList = new List<UserDto>();

            _userRoleServiceMock
                .Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(emptyList);
            _userRoleServiceMock.Verify(x => x.GetAllUsersAsync(), Times.Once);
        }

        // ==================== GetUserById ====================

        /// <summary>
        /// Проверяет, что при существующем ID пользователя возвращается 200 OK и сам пользователь.
        /// </summary>
        [Fact]
        public async Task GetUserById_WhenUserExists_ReturnsOkWithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new UserDto
            {
                Id = userId.ToString(),
                UserName = "Alice",
                Email = "alice@example.com",
                Roles = new[] { "Admin" }
            };

            _userRoleServiceMock
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedUser);
            _userRoleServiceMock.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при несуществующем ID пользователя возвращается 404 NotFound.
        /// </summary>
        [Fact]
        public async Task GetUserById_WhenUserNotFound_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRoleServiceMock
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _userRoleServiceMock.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
        }

        // ==================== ChangeUserRole ====================

        /// <summary>
        /// Проверяет, что при успешной смене роли пользователя возвращается 200 OK.
        /// </summary>
        [Fact]
        public async Task ChangeUserRole_WhenUserExists_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();

            _userRoleServiceMock
                .Setup(x => x.ChangeUserRoleAsync(userId, roleId))
                .ReturnsAsync(new Models.Responses.ChangeUserPropertiesResponse { Success = true });

            // Act
            var result = await _controller.ChangeUserRole(userId, roleId);

            // Assert
            result.Should().BeOfType<OkResult>();
            _userRoleServiceMock.Verify(x => x.ChangeUserRoleAsync(userId, roleId), Times.Once);
        }

        /// <summary>
        /// Проверяет, что при попытке сменить роль несуществующего пользователя сервис выбрасывает
        /// KeyNotFoundException, и контроллер пробрасывает это исключение (не перехватывает).
        /// </summary>
        [Fact]
        public async Task ChangeUserRole_WhenUserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            const string errorMessage = "User not found";

            _userRoleServiceMock
                .Setup(x => x.ChangeUserRoleAsync(userId, roleId))
                .ThrowsAsync(new KeyNotFoundException(errorMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _controller.ChangeUserRole(userId, roleId)
            );
            exception.Message.Should().Be(errorMessage);
        }
    }
}
