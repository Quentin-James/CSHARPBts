using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupesSpectaclesController : ControllerBase
    {
        private readonly IGroupeSpectacleService _groupeSpectacleService;

        public GroupesSpectaclesController(IGroupeSpectacleService groupeSpectacleService)
        {
            _groupeSpectacleService = groupeSpectacleService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var groupeSpectacle = await _groupeSpectacleService.GetByIdAsync(id);
            if (groupeSpectacle == null)
            {
                return NotFound();
            }
            return Ok(groupeSpectacle);
        }
    }
}

