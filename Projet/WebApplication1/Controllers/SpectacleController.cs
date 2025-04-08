using Microsoft.AspNetCore.Mvc;
using DAL.Modeles;
using Services.DTOs;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpectacleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SpectacleController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> CreateSpectacle(SpectacleDto spectacleDto)
        {
            if (spectacleDto == null)
                return BadRequest("Les données du spectacle sont invalides");

            var spectacle = new Spectacle
            {
                Titre = spectacleDto.Titre,
                Description = spectacleDto.Description,
                Type = spectacleDto.Type,
                Duree = spectacleDto.Duree
            };

            _context.Spectacles.Add(spectacle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateSpectacle), new { id = spectacle.SpectacleId }, spectacle);
        }
    }
}
