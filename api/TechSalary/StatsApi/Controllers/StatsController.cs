using Microsoft.AspNetCore.Mvc;
using StatsApi.Data;

namespace StatsApi.Controllers
{
    [ApiController]
    [Route("api/stats")]
    public class StatsController : ControllerBase
    {
        private readonly IStatsRepository _repository;

        public StatsController(IStatsRepository repository)
        {
            _repository = repository;
        }

        // GET /api/stats?role=Engineer&country=SriLanka&company=Google&experienceLevel=Mid
        [HttpGet]
        public async Task<IActionResult> GetStats(
            [FromQuery] string? role,
            [FromQuery] string? country,
            [FromQuery] string? company,
            [FromQuery] string? experienceLevel)
        {
            var stats = await _repository.CalculateStatsAsync(role, country, company, experienceLevel);
            
            if (stats == null)
                return NotFound("No approved data found for these filters.");

            return Ok(stats);
        }
    }
}