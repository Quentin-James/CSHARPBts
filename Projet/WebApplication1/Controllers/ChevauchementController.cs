using Microsoft.AspNetCore.Mvc;
using Services.DTOs;
using Services.Interfaces;

namespace WebApplication1.Controllers
{
    [Route("api/spectacles/chevauchements")]
    [ApiController]
    public class ChevauchementController : ControllerBase
    {
        private readonly IChevauchementService _chevauchementService;

        public ChevauchementController(IChevauchementService chevauchementService)
        {
            _chevauchementService = chevauchementService;
        }

        [HttpGet("check-table")]
        public async Task<ActionResult> CheckTableName()
        {
            try
            {
                var tables = await _chevauchementService.GetTableNamesAsync();
                return Ok(new { Tables = tables });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<ChevauchementDto>>> GetChevauchements()
        {
            try
            {
                var chevauchements = await _chevauchementService.GetChevauchementsAsync();
                return Ok(chevauchements);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("test-data")]
        public async Task<ActionResult> InsertTestData()
        {
            try
            {
                await _chevauchementService.InsertTestDataAsync();
                return Ok("Données de test insérées avec succès");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
} 