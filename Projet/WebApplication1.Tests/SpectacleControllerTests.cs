using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Interfaces;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class SpectacleControllerTests
    {
        private readonly Mock<ISpectacleService> _mockSpectacleService;
        private readonly SpectacleController _controller;

        public SpectacleControllerTests()
        {
            _mockSpectacleService = new Mock<ISpectacleService>();
            _controller = new SpectacleController(_mockSpectacleService.Object);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var spectacleId = 1;
            var mockSpectacle = new DAL.Modeles.Spectacle
            {
                SpectacleId = spectacleId,
                Titre = "Roméo et Juliette",
                Description = "Tragédie de Shakespeare",
                Type = "Théâtre",
                Duree = new TimeOnly(2, 30),
                Saison = "2024"
            };

            _mockSpectacleService.Setup(x => x.GetByIdAsync(spectacleId))
                .ReturnsAsync(mockSpectacle);

            // Act
            var result = await _controller.GetById(spectacleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSpectacle = Assert.IsType<DAL.Modeles.Spectacle>(okResult.Value);
            Assert.Equal(spectacleId, returnedSpectacle.SpectacleId);
            Assert.Equal("Roméo et Juliette", returnedSpectacle.Titre);
            Assert.Equal("Tragédie de Shakespeare", returnedSpectacle.Description);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var spectacleId = 999;
            _mockSpectacleService.Setup(x => x.GetByIdAsync(spectacleId))
                .ReturnsAsync((DAL.Modeles.Spectacle?)null);

            // Act
            var result = await _controller.GetById(spectacleId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_WithException_ThrowsException()
        {
            // Arrange
            var spectacleId = 1;
            _mockSpectacleService.Setup(x => x.GetByIdAsync(spectacleId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetById(spectacleId));
        }
    }
} 