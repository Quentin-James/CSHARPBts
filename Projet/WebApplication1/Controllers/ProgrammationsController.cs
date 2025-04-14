using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgrammationsController : ControllerBase
    {
        private readonly IProgrammationService _programmationService;

        public ProgrammationsController(IProgrammationService programmationService)
        {
            _programmationService = programmationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var programmation = await _programmationService.GetByIdAsync(id);
            if (programmation == null)
            {
                return NotFound();
            }
            return Ok(programmation);
        }
    }
}
