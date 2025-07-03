using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.DTOs;
using Services.Interfaces;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class BilletsControllerTests
    {
        private readonly Mock<IBilletService> _mockBilletService;
        private readonly BilletsController _controller;

        public BilletsControllerTests()
        {
            _mockBilletService = new Mock<IBilletService>();
            _controller = new BilletsController(_mockBilletService.Object);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var billetId = 1;
            var mockBillet = new DAL.Modeles.Billet
            {
                BilletId = billetId,
                Nom = "Dupont",
                Prenom = "Jean",
                PrixAchat = 25.50m,
                Programmation = new DAL.Modeles.Programmation
                {
                    Date = new DateOnly(2024, 1, 15),
                    Heure = new TimeOnly(20, 0),
                    Spectacle = new DAL.Modeles.Spectacle
                    {
                        Titre = "Roméo et Juliette"
                    }
                }
            };

            _mockBilletService.Setup(x => x.GetByIdAsync(billetId))
                .ReturnsAsync(mockBillet);

            // Act
            var result = await _controller.GetById(billetId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BilletAnonymous>(okResult.Value);
            Assert.True(response.valide);
            Assert.Equal(billetId, response.billet.id);
            Assert.Equal("Roméo et Juliette", response.billet.spectacle);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var billetId = 999;
            _mockBilletService.Setup(x => x.GetByIdAsync(billetId))
                .ReturnsAsync((DAL.Modeles.Billet?)null);

            // Act
            var result = await _controller.GetById(billetId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<BilletAnonymous>(notFoundResult.Value);
            Assert.Equal("Billet non trouvé", response.message);
        }

        [Fact]
        public async Task GetFrequentationBySpectacle_WithValidId_ReturnsFileResult()
        {
            // Arrange
            var spectacleId = 1;
            var frequentationData = new List<FrequentationDTO>
            {
                new FrequentationDTO
                {
                    SpectacleId = spectacleId,
                    TitreSpectacle = "Roméo et Juliette",
                    DateRepresentation = new DateOnly(2024, 1, 15),
                    NombreBilletsVendus = 25
                },
                new FrequentationDTO
                {
                    SpectacleId = spectacleId,
                    TitreSpectacle = "Roméo et Juliette",
                    DateRepresentation = new DateOnly(2024, 1, 20),
                    NombreBilletsVendus = 30
                }
            };

            _mockBilletService.Setup(x => x.GetFrequentationBySpectacleAsync(spectacleId))
                .ReturnsAsync(frequentationData);

            // Act
            var result = await _controller.GetFrequentationBySpectacle(spectacleId);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal($"frequentation_spectacle_{spectacleId}.csv", fileResult.FileDownloadName);
            
            var csvContent = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
            Assert.Contains("id_spectacle,titre_spectacle,date_representation,nombre_billets_vendus", csvContent);
            Assert.Contains("1,\"Roméo et Juliette\",2024-01-15,25", csvContent);
            Assert.Contains("1,\"Roméo et Juliette\",2024-01-20,30", csvContent);
        }

        [Fact]
        public async Task GetFrequentationBySpectacle_WithNoData_ReturnsNotFound()
        {
            // Arrange
            var spectacleId = 999;
            _mockBilletService.Setup(x => x.GetFrequentationBySpectacleAsync(spectacleId))
                .ReturnsAsync(new List<FrequentationDTO>());

            // Act
            var result = await _controller.GetFrequentationBySpectacle(spectacleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<BilletAnonymous>(notFoundResult.Value);
            Assert.Equal("Aucune représentation trouvée pour ce spectacle", response.message);
        }

        [Fact]
        public async Task GetFrequentationBySpectacle_WithException_ReturnsInternalServerError()
        {
            // Arrange
            var spectacleId = 1;
            _mockBilletService.Setup(x => x.GetFrequentationBySpectacleAsync(spectacleId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetFrequentationBySpectacle(spectacleId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var response = Assert.IsType<BilletAnonymous>(statusCodeResult.Value);
            Assert.Equal("Erreur lors de la récupération des données de fréquentation", response.message);
            Assert.Equal("Database error", response.error);
        }
    }

    // Classe anonyme pour les assertions
    public class BilletAnonymous
    {
        public bool valide { get; set; }
        public BilletInfo billet { get; set; } = new();
        public string message { get; set; } = string.Empty;
        public string error { get; set; } = string.Empty;
    }

    public class BilletInfo
    {
        public int id { get; set; }
        public string spectacle { get; set; } = string.Empty;
        public DateOnly? date { get; set; }
        public TimeOnly? heure { get; set; }
    }
} 