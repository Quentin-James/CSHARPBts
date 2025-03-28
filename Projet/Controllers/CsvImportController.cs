using Microsoft.AspNetCore.Mvc;
using Services.CsvImport;
using System.IO;
using System.Threading.Tasks;

namespace Controllers
{
    /// <summary>
    /// Controller for importing CSV files.
    /// </summary>
    [ApiController]
    [Route("api/csv-import")]
    public class CsvImportController : ControllerBase
    {
        private readonly ICsvImportService _csvImportService;

        public CsvImportController(ICsvImportService csvImportService)
        {
            _csvImportService = csvImportService;
        }

        /// <summary>
        /// Imports a CSV file containing Spectacles.
        /// </summary>
        /// <param name="file">The CSV file to import.</param>
        /// <returns>Success or error message.</returns>
        [HttpPost("spectacles")]
        public async Task<IActionResult> ImportSpectacles([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            await _csvImportService.ImportSpectaclesAsync(filePath);
            return Ok("Spectacles imported successfully");
        }

        /// <summary>
        /// Imports a CSV file containing Billets.
        /// </summary>
        /// <param name="file">The CSV file to import.</param>
        /// <returns>Success or error message.</returns>
        [HttpPost("billets")]
        public async Task<IActionResult> ImportBillets([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            await _csvImportService.ImportBilletsAsync(filePath);
            return Ok("Billets imported successfully");
        }
    }
}
