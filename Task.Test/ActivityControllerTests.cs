using Activity.API.ApiModel;
using Activity.API.Controllers;
using Activity.API.Entities;
using Activity.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Activity.Test
{
    public class ActivityControllerTests
    {
        private readonly Mock<IActivityTicketRepository> _mockTaskRepository;
        private readonly ActivityTicketController _activityController;

        public ActivityControllerTests()
        {
            _mockTaskRepository = new Mock<IActivityTicketRepository>();
            _activityController = new ActivityTicketController(_mockTaskRepository.Object, null);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAll_ReturnsOkObjectResult_WithListOfTasks()
        {
            // Arrange
            var expectedTasks = new List<ActivityTicket> { new ActivityTicket(), new ActivityTicket() };
            _mockTaskRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedTasks);

            // Act
            var result = await _activityController.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTasks = Assert.IsType<List<ActivityTicket>>(okResult.Value);
            Assert.Equal(expectedTasks, actualTasks);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetById_WithValidId_ReturnsOkObjectResult_WithTask()
        {
            // Arrange
            var expectedTask = new ActivityTicket();
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedTask);

            // Act
            var result = await _activityController.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualTask = Assert.IsType<ActivityTicket>(okResult.Value);
            Assert.Equal(expectedTask, actualTask);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsOkObjectResult_WithNull()
        {
            // Arrange
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(null as ActivityTicket);

            // Act
            var result = await _activityController.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(okResult.Value);
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_WithValidModel_ReturnsOkObjectResult_WithMessage()
        {
            // Arrange
            var model = new ActivityTicketApiModel();
            var expectedMessage = "ActivityTicket created successfully";
            _mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<ActivityTicket>())).ReturnsAsync(true);

            // Act
            var result = await _activityController.Create(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualMessage = Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async System.Threading.Tasks.Task Create_WithInvalidModel_ReturnsBadRequestObjectResult_WithMessage()
        {
            // Arrange
            var model = new ActivityTicketApiModel();
            var expectedMessage = "ActivityTicket creation failed";
            _mockTaskRepository.Setup(repo => repo.AddAsync(It.IsAny<ActivityTicket>())).ReturnsAsync(false);

            // Act
            var result = await _activityController.Create(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var actualMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_WithValidId_ReturnsOkObjectResult_WithMessage()
        {
            // Arrange
            var expectedMessage = "ActivityTicket deleted successfully";
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new ActivityTicket());
            _mockTaskRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _activityController.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualMessage = Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async System.Threading.Tasks.Task Delete_WithInvalidId_ReturnsBadRequestObjectResult_WithMessage()
        {
            // Arrange
            var expectedMessage = "ActivityTicket not found";
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(null as ActivityTicket);

            // Act
            var result = await _activityController.Delete(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var actualMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task Delete_WithValidIdButDeleteFails_ReturnsBadRequestObjectResult_WithMessage()
        {
            // Arrange
            var expectedMessage = "ActivityTicket deletion failed";
            _mockTaskRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new ActivityTicket());
            _mockTaskRepository.Setup(repo => repo.DeleteAsync(It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _activityController.Delete(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var actualMessage = Assert.IsType<string>(badRequestResult.Value);
            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
