using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Text;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BilletsController : ControllerBase
    {
        private readonly IBilletService _billetService;

        public BilletsController(IBilletService billetService)
        {
            _billetService = billetService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var billet = await _billetService.GetByIdAsync(id);
            if (billet == null)
            {
                return NotFound(new { message = "Billet non trouvé" });
            }
            return Ok(new { 
                valide = true,
                billet = new {
                    id = billet.BilletId,
                    spectacle = billet.Programmation?.Spectacle?.Titre,
                    date = billet.Programmation?.Date,
                    heure = billet.Programmation?.Heure
                }
            });
        }

        [HttpGet("frequentation/{spectacleId}")]
        public async Task<IActionResult> GetFrequentationBySpectacle(int spectacleId)
        {
            try
            {
                var frequentation = await _billetService.GetFrequentationBySpectacleAsync(spectacleId);
                
                if (!frequentation.Any())
                {
                    return NotFound(new { message = "Aucune représentation trouvée pour ce spectacle" });
                }

                var csvContent = new StringBuilder();
                csvContent.AppendLine("id_spectacle,titre_spectacle,date_representation,nombre_billets_vendus");
                
                foreach (var item in frequentation)
                {
                    csvContent.AppendLine($"{item.SpectacleId},\"{item.TitreSpectacle}\",{item.DateRepresentation:yyyy-MM-dd},{item.NombreBilletsVendus}");
                }

                var bytes = Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(bytes, "text/csv", $"frequentation_spectacle_{spectacleId}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des données de fréquentation", error = ex.Message });
            }
        }
    }
}
