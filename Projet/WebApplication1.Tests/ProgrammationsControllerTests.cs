using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Interfaces;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class ProgrammationsControllerTests
    {
        private readonly Mock<IProgrammationService> _mockProgrammationService;
        private readonly ProgrammationsController _controller;

        public ProgrammationsControllerTests()
        {
            _mockProgrammationService = new Mock<IProgrammationService>();
            _controller = new ProgrammationsController(_mockProgrammationService.Object);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var programmationId = 1;
            var mockProgrammation = new DAL.Modeles.Programmation
            {
                ProgrammationId = programmationId,
                Date = new DateOnly(2024, 1, 15),
                Heure = new TimeOnly(20, 0),
                Lieu = "Grand Théâtre",
                SpectacleId = 1,
                Spectacle = new DAL.Modeles.Spectacle
                {
                    SpectacleId = 1,
                    Titre = "Roméo et Juliette"
                }
            };

            _mockProgrammationService.Setup(x => x.GetByIdAsync(programmationId))
                .ReturnsAsync(mockProgrammation);

            // Act
            var result = await _controller.GetById(programmationId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProgrammation = Assert.IsType<DAL.Modeles.Programmation>(okResult.Value);
            Assert.Equal(programmationId, returnedProgrammation.ProgrammationId);
            Assert.Equal(new DateOnly(2024, 1, 15), returnedProgrammation.Date);
            Assert.Equal(new TimeOnly(20, 0), returnedProgrammation.Heure);
            Assert.Equal("Grand Théâtre", returnedProgrammation.Lieu);
            Assert.Equal("Roméo et Juliette", returnedProgrammation.Spectacle?.Titre);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var programmationId = 999;
            _mockProgrammationService.Setup(x => x.GetByIdAsync(programmationId))
                .ReturnsAsync((DAL.Modeles.Programmation?)null);

            // Act
            var result = await _controller.GetById(programmationId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_WithException_ThrowsException()
        {
            // Arrange
            var programmationId = 1;
            _mockProgrammationService.Setup(x => x.GetByIdAsync(programmationId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetById(programmationId));
        }
    }
} 