using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services.CsvImport;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// Controller for importing CSV files.
    /// </summary>
    [ApiController]
    [Route("api/csv-import")]
    public class ImportController : ControllerBase
    {
        private readonly ICsvImportService _csvImportService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(ICsvImportService csvImportService, ILogger<ImportController> logger)
        {
            _csvImportService = csvImportService;
            _logger = logger;
        }

    
        [HttpPost("spectacles")]
        public async Task<IActionResult> ImportSpectacles(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Le fichier est vide");

            var filePath = Path.GetTempFileName();
            try
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                await _csvImportService.ImportSpectaclesAsync(filePath);
                return Ok("Spectacles importés avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur d'importation des spectacles");
                return StatusCode(500, $"Erreur: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }

    
        [HttpPost("billets")]
        public async Task<IActionResult> ImportBillets(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Le fichier est vide");

            var filePath = Path.GetTempFileName();
            try
            {
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                await _csvImportService.ImportBilletsAsync(filePath);
                return Ok("Billets importés avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur d'importation des billets");
                return StatusCode(500, $"Erreur: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }
    }
}
