using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpectacleController : ControllerBase
    {
        private readonly ISpectacleService _spectacleService;

        public SpectacleController(ISpectacleService spectacleService)
        {
            _spectacleService = spectacleService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var spectacle = await _spectacleService.GetByIdAsync(id);
            if (spectacle == null)
            {
                return NotFound();
            }
            return Ok(spectacle);
        }
    }
} 