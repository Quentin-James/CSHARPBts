using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Modeles;
using Services.DTOs;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypesTarifsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TypesTarifsController(AppDbContext context) => _context = context;

        [HttpPost]
        public async Task<ActionResult<TypesTarifDto>> CreateTypeTarif(TypesTarifDto typesTarifDto)
        {
            if (typesTarifDto == null)
                return BadRequest("Les données du tarif sont invalides");

            var typeTarif = new TypesTarif { NomTarif = typesTarifDto.NomTarif };

            _context.TypesTarifs.Add(typeTarif);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateTypeTarif), 
                new { id = typeTarif.TarifId }, typesTarifDto);
        }
    }
}
