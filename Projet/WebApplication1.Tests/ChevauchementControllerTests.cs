using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.DTOs;
using Services.Interfaces;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class ChevauchementControllerTests
    {
        private readonly Mock<IChevauchementService> _mockChevauchementService;
        private readonly ChevauchementController _controller;

        public ChevauchementControllerTests()
        {
            _mockChevauchementService = new Mock<IChevauchementService>();
            _controller = new ChevauchementController(_mockChevauchementService.Object);
        }

        [Fact]
        public async Task CheckTableName_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var tableNames = new List<string> { "Spectacles", "Billets", "Programmations" };
            _mockChevauchementService.Setup(x => x.GetTableNamesAsync())
                .ReturnsAsync(tableNames);

            // Act
            var result = await _controller.CheckTableName();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ChevauchementAnonymous>(okResult.Value);
            Assert.Equal(tableNames, response.Tables);
        }

        [Fact]
        public async Task CheckTableName_WithException_ReturnsBadRequest()
        {
            // Arrange
            _mockChevauchementService.Setup(x => x.GetTableNamesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CheckTableName();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ChevauchementAnonymous>(badRequestResult.Value);
            Assert.Equal("Database error", response.Error);
        }

        [Fact]
        public async Task GetChevauchements_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var chevauchements = new List<ChevauchementDto>
            {
                new ChevauchementDto
                {
                    SpectacleId = 1,
                    Titre = "Roméo et Juliette",
                    Date = new DateOnly(2024, 1, 15),
                    HeureDebut = new TimeOnly(20, 0),
                    SpectacleChevaucheId = 2,
                    TitreChevauchement = "Hamlet"
                }
            };

            _mockChevauchementService.Setup(x => x.GetChevauchementsAsync())
                .ReturnsAsync(chevauchements);

            // Act
            var result = await _controller.GetChevauchements();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChevauchements = Assert.IsType<List<ChevauchementDto>>(okResult.Value);
            Assert.Single(returnedChevauchements);
            Assert.Equal(1, returnedChevauchements[0].SpectacleId);
            Assert.Equal("Roméo et Juliette", returnedChevauchements[0].Titre);
        }

        [Fact]
        public async Task GetChevauchements_WithException_ReturnsBadRequest()
        {
            // Arrange
            _mockChevauchementService.Setup(x => x.GetChevauchementsAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetChevauchements();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ChevauchementAnonymous>(badRequestResult.Value);
            Assert.Equal("Database error", response.Error);
        }

        [Fact]
        public async Task InsertTestData_WithValidData_ReturnsOkResult()
        {
            // Arrange
            _mockChevauchementService.Setup(x => x.InsertTestDataAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.InsertTestData();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Données de test insérées avec succès", okResult.Value);
        }

        [Fact]
        public async Task InsertTestData_WithException_ReturnsBadRequest()
        {
            // Arrange
            _mockChevauchementService.Setup(x => x.InsertTestDataAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.InsertTestData();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ChevauchementAnonymous>(badRequestResult.Value);
            Assert.Equal("Database error", response.Error);
        }
    }

    public class ChevauchementAnonymous
    {
        public List<string> Tables { get; set; } = new();
        public string Error { get; set; } = string.Empty;
    }
} 