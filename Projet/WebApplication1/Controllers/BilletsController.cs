using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
    }
}
