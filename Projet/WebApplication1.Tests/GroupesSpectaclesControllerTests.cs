using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Interfaces;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class GroupesSpectaclesControllerTests
    {
        private readonly Mock<IGroupeSpectacleService> _mockGroupeSpectacleService;
        private readonly GroupesSpectaclesController _controller;

        public GroupesSpectaclesControllerTests()
        {
            _mockGroupeSpectacleService = new Mock<IGroupeSpectacleService>();
            _controller = new GroupesSpectaclesController(_mockGroupeSpectacleService.Object);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var groupeSpectacleId = 1;
            var mockGroupeSpectacle = new DAL.Modeles.GroupesSpectacle
            {
                GroupeId = groupeSpectacleId,
                NomGroupe = "Groupe A"
            };

            _mockGroupeSpectacleService.Setup(x => x.GetByIdAsync(groupeSpectacleId))
                .ReturnsAsync(mockGroupeSpectacle);

            // Act
            var result = await _controller.GetById(groupeSpectacleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGroupeSpectacle = Assert.IsType<DAL.Modeles.GroupesSpectacle>(okResult.Value);
            Assert.Equal(groupeSpectacleId, returnedGroupeSpectacle.GroupeId);
            Assert.Equal("Groupe A", returnedGroupeSpectacle.NomGroupe);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var groupeSpectacleId = 999;
            _mockGroupeSpectacleService.Setup(x => x.GetByIdAsync(groupeSpectacleId))
                .ReturnsAsync((DAL.Modeles.GroupesSpectacle?)null);

            // Act
            var result = await _controller.GetById(groupeSpectacleId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_WithException_ThrowsException()
        {
            // Arrange
            var groupeSpectacleId = 1;
            _mockGroupeSpectacleService.Setup(x => x.GetByIdAsync(groupeSpectacleId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetById(groupeSpectacleId));
        }
    }
} 