using Microsoft.AspNetCore.Mvc;
using StatsApi.Data;
using StatsApi.Models;

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

            if (stats == null) stats = new StatsResponse
            {
                AverageSalary = 0,
                MedianSalary = 0,
                P25Salary = 0,
                P75Salary = 0,
                RecordCount = 0,
                GeneratedAt = DateTime.UtcNow,
                Role = role,
                Country = country,
                Company = company,
                ExperienceLevel = experienceLevel
            };
            return Ok(stats);
        }
    }
}