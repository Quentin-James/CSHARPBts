using Microsoft.AspNetCore.Mvc;
using Services.CsvImport;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/csv-import")]
    public class CsvImportController : ControllerBase
    {
        private readonly ICsvImportService _csvImportService;

        public CsvImportController(ICsvImportService csvImportService)
        {
            _csvImportService = csvImportService;
        }

        [HttpPost("spectacles")]
        public async Task<IActionResult> ImportSpectacles(IFormFile file)
        {
            string filePath = string.Empty;
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Aucun fichier n'a été envoyé.");

                filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                await _csvImportService.ImportSpectaclesAsync(filePath);
                return Ok("Import des spectacles réussi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'import : {ex.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }

        [HttpPost("billets")]
        public async Task<IActionResult> ImportBillets(IFormFile file)
        {
            string filePath = string.Empty;
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Aucun fichier n'a été envoyé.");

                filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                await _csvImportService.ImportBilletsAsync(filePath);
                return Ok("Import des billets réussi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'import : {ex.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }
    }
} 