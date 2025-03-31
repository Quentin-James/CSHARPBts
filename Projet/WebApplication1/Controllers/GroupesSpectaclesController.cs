using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Modeles;
using Services.DTOs;
using System.Threading.Tasks;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupesSpectaclesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupesSpectaclesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<GroupesSpectacleDto>> CreateGroupesSpectacle([FromBody] GroupesSpectacleDto groupesSpectacleDto)
        {
            if (groupesSpectacleDto == null)
                return BadRequest("Les données du groupe de spectacle sont invalides.");

            var tarifExiste = await _context.TypesTarifs.AnyAsync(t => t.TarifId == groupesSpectacleDto.TarifId);
            var programmationExiste = await _context.Programmations.AnyAsync(p => p.ProgrammationId == groupesSpectacleDto.ProgrammationId);

            if (!tarifExiste)
                return BadRequest("Le tarif spécifié n'existe pas.");
            if (!programmationExiste)
                return BadRequest("La programmation spécifiée n'existe pas.");

            var groupesSpectacle = new GroupesSpectacle
            {
                NomGroupe = groupesSpectacleDto.NomGroupe
            };

            _context.GroupesSpectacles.Add(groupesSpectacle);
            await _context.SaveChangesAsync();

            groupesSpectacleDto.GroupeId = groupesSpectacle.GroupeId; // Mise à jour du DTO avec l'ID généré

            return CreatedAtAction(nameof(CreateGroupesSpectacle), new { id = groupesSpectacle.GroupeId }, groupesSpectacleDto);
        }
    }
}

