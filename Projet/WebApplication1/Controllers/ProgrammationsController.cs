using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Modeles;
using Services.DTOs;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgrammationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgrammationsController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<ProgrammationDto>> CreateProgrammation(ProgrammationDto programmationDto)
        {
            if (programmationDto == null)
                return BadRequest("Les données de programmation sont invalides");

            var spectacleExiste = await _context.Spectacles.AnyAsync(s => s.SpectacleId == programmationDto.SpectacleId);
            if (!spectacleExiste)
                return BadRequest("Spectacle inexistant");

            var programmation = new Programmation
            {
                Date = programmationDto.Date,
                Heure = programmationDto.Heure,
                Lieu = programmationDto.Lieu,
                SpectacleId = programmationDto.SpectacleId
            };

            _context.Programmations.Add(programmation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateProgrammation), 
                new { id = programmation.ProgrammationId }, programmationDto);
        }
    }
}
