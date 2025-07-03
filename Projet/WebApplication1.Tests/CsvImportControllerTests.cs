using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Services.CsvImport;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests
{
    public class CsvImportControllerTests
    {
        private readonly Mock<ICsvImportService> _mockCsvImportService;
        private readonly CsvImportController _controller;

        public CsvImportControllerTests()
        {
            _mockCsvImportService = new Mock<ICsvImportService>();
            _controller = new CsvImportController(_mockCsvImportService.Object);
        }

        [Fact]
        public async Task ImportSpectacles_WithValidFile_ReturnsOkResult()
        {
            // Arrange
            var fileContent = "Titre,Description,Type,Duree,Saison\nRoméo et Juliette,Tragédie,Théâtre,02:30,2024";
            var file = CreateMockFile(fileContent, "spectacles.csv");

            _mockCsvImportService.Setup(x => x.ImportSpectaclesAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ImportSpectacles(file);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Import des spectacles réussi.", okResult.Value);
        }

        [Fact]
        public async Task ImportSpectacles_WithNullFile_ReturnsBadRequest()
        {
            // Arrange
            IFormFile? file = null;

            // Act
            var result = await _controller.ImportSpectacles(file);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Aucun fichier n'a été envoyé.", badRequestResult.Value);
        }

        [Fact]
        public async Task ImportSpectacles_WithEmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var file = CreateMockFile("", "spectacles.csv");

            // Act
            var result = await _controller.ImportSpectacles(file);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Aucun fichier n'a été envoyé.", badRequestResult.Value);
        }

        [Fact]
        public async Task ImportSpectacles_WithException_ReturnsInternalServerError()
        {
            // Arrange
            var fileContent = "Titre,Description,Type,Duree,Saison\nRoméo et Juliette,Tragédie,Théâtre,02:30,2024";
            var file = CreateMockFile(fileContent, "spectacles.csv");

            _mockCsvImportService.Setup(x => x.ImportSpectaclesAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Import error"));

            // Act
            var result = await _controller.ImportSpectacles(file);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erreur lors de l'import : Import error", statusCodeResult.Value);
        }

        [Fact]
        public async Task ImportBillets_WithValidFile_ReturnsOkResult()
        {
            // Arrange
            var fileContent = "Civilite,Nom,Prenom,PrixAchat,TarifId,ProgrammationId\nM,Dupont,Jean,25.50,1,1";
            var file = CreateMockFile(fileContent, "billets.csv");

            _mockCsvImportService.Setup(x => x.ImportBilletsAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ImportBillets(file);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Import des billets réussi.", okResult.Value);
        }

        [Fact]
        public async Task ImportBillets_WithNullFile_ReturnsBadRequest()
        {
            // Arrange
            IFormFile? file = null;

            // Act
            var result = await _controller.ImportBillets(file);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Aucun fichier n'a été envoyé.", badRequestResult.Value);
        }

        [Fact]
        public async Task ImportBillets_WithEmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var file = CreateMockFile("", "billets.csv");

            // Act
            var result = await _controller.ImportBillets(file);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Aucun fichier n'a été envoyé.", badRequestResult.Value);
        }

        [Fact]
        public async Task ImportBillets_WithException_ReturnsInternalServerError()
        {
            // Arrange
            var fileContent = "Civilite,Nom,Prenom,PrixAchat,TarifId,ProgrammationId\nM,Dupont,Jean,25.50,1,1";
            var file = CreateMockFile(fileContent, "billets.csv");

            _mockCsvImportService.Setup(x => x.ImportBilletsAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Import error"));

            // Act
            var result = await _controller.ImportBillets(file);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erreur lors de l'import : Import error", statusCodeResult.Value);
        }

        private static IFormFile CreateMockFile(string content, string fileName)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(bytes.Length);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => stream.WriteAsync(bytes, 0, bytes.Length, token));

            return mockFile.Object;
        }
    }
} 