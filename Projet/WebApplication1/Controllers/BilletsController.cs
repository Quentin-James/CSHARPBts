using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BilletsController : ControllerBase
    {
        private readonly IBilletService _billetService;

        public BilletsController(IBilletService billetService)
        {
            _billetService = billetService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var billet = await _billetService.GetByIdAsync(id);
            if (billet == null)
            {
                return NotFound();
            }
            return Ok(billet);
        }
    }
}
