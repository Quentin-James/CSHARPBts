using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypesTarifsController : ControllerBase
    {
        private readonly ITypesTarifService _typesTarifService;

        public TypesTarifsController(ITypesTarifService typesTarifService)
        {
            _typesTarifService = typesTarifService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var typeTarif = await _typesTarifService.GetByIdAsync(id);
            if (typeTarif == null)
            {
                return NotFound();
            }
            return Ok(typeTarif);
        }
    }
}
