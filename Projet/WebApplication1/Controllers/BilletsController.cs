using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Modeles;
using Services.DTOs;
using System.Threading.Tasks;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BilletsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BilletsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<BilletDTO>> CreateBillet([FromBody] BilletDTO billetDto)
        {
            if (billetDto == null)
                return BadRequest("Les données du billet sont invalides.");

            // Vérification si le tarif et la programmation existent
            var tarifExiste = await _context.TypesTarifs.AnyAsync(t => t.TarifId == billetDto.TarifId);
            var programmationExiste = await _context.Programmations.AnyAsync(p => p.ProgrammationId == billetDto.ProgrammationId);

            if (!tarifExiste)
                return BadRequest("Le tarif spécifié n'existe pas.");
            if (!programmationExiste)
                return BadRequest("La programmation spécifiée n'existe pas.");

            var billet = new Billet
            {
                Civilite = billetDto.Civilite,
                Nom = billetDto.Nom,
                Prenom = billetDto.Prenom,
                PrixAchat = billetDto.PrixAchat,
                TarifId = billetDto.TarifId,
                ProgrammationId = billetDto.ProgrammationId
            };

            _context.Billets.Add(billet);
            await _context.SaveChangesAsync(); // Sauvegarde en base

            return CreatedAtAction(nameof(CreateBillet), new { id = billet.BilletId }, billetDto);
        }
    }
}
