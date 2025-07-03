using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.Interfaces;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class TypeTarifsControllerTests
    {
        private readonly Mock<ITypesTarifService> _mockTypesTarifService;
        private readonly TypesTarifsController _controller;

        public TypeTarifsControllerTests()
        {
            _mockTypesTarifService = new Mock<ITypesTarifService>();
            _controller = new TypesTarifsController(_mockTypesTarifService.Object);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var typeTarifId = 1;
            var mockTypeTarif = new DAL.Modeles.TypesTarif
            {
                TarifId = typeTarifId,
                NomTarif = "Plein tarif"
            };

            _mockTypesTarifService.Setup(x => x.GetByIdAsync(typeTarifId))
                .ReturnsAsync(mockTypeTarif);

            // Act
            var result = await _controller.GetById(typeTarifId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTypeTarif = Assert.IsType<DAL.Modeles.TypesTarif>(okResult.Value);
            Assert.Equal(typeTarifId, returnedTypeTarif.TarifId);
            Assert.Equal("Plein tarif", returnedTypeTarif.NomTarif);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var typeTarifId = 999;
            _mockTypesTarifService.Setup(x => x.GetByIdAsync(typeTarifId))
                .ReturnsAsync((DAL.Modeles.TypesTarif?)null);

            // Act
            var result = await _controller.GetById(typeTarifId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_WithException_ThrowsException()
        {
            // Arrange
            var typeTarifId = 1;
            _mockTypesTarifService.Setup(x => x.GetByIdAsync(typeTarifId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetById(typeTarifId));
        }
    }
} 