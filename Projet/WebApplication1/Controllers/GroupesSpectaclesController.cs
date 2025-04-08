using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Modeles;
using Services.DTOs;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupesSpectaclesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupesSpectaclesController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<GroupesSpectacleDto>> CreateGroupesSpectacle(GroupesSpectacleDto dto)
        {
            if (dto == null)
                return BadRequest("Les données du groupe sont invalides");

            var tarifExiste = await _context.TypesTarifs.AnyAsync(t => t.TarifId == dto.TarifId);
            var programmationExiste = await _context.Programmations.AnyAsync(p => p.ProgrammationId == dto.ProgrammationId);

            if (!tarifExiste)
                return BadRequest("Tarif inexistant");
                
            if (!programmationExiste)
                return BadRequest("Programmation inexistante");

            var groupe = new GroupesSpectacle { NomGroupe = dto.NomGroupe };
            
            _context.GroupesSpectacles.Add(groupe);
            await _context.SaveChangesAsync();

            dto.GroupeId = groupe.GroupeId;
            return CreatedAtAction(nameof(CreateGroupesSpectacle), new { id = groupe.GroupeId }, dto);
        }
    }
}

