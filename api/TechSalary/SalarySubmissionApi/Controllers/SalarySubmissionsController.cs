using Microsoft.AspNetCore.Mvc;
using SalarySubmissionApi.Data;
using SalarySubmissionApi.Models;

namespace SalarySubmissionApi.Controllers
{
    [ApiController]
    [Route("api/salaries")]
    public class SalarySubmissionsController : ControllerBase
    {
        private readonly SalaryRepository _repository;

        public SalarySubmissionsController(SalaryRepository repository)
        {
            _repository = repository;
        }

        // POST /api/salaries
        // Anonymous salary submission
        [HttpPost]
        public async Task<IActionResult> SubmitSalary([FromBody] SalarySubmissionRequest request)
        {
            if (request.SalaryAmount <= 0)
                return BadRequest("Salary must be greater than zero");

            if (string.IsNullOrWhiteSpace(request.Country) ||
                string.IsNullOrWhiteSpace(request.Company) ||
                string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Missing required fields");

            var submission = new SalarySubmission
            {
                Id = Guid.NewGuid(),
                Country = request.Country,
                Company = request.Company,
                Role = request.Role,
                ExperienceLevel = request.ExperienceLevel,
                SalaryAmount = request.SalaryAmount,
                Currency = request.Currency,
                Anonymize = request.Anonymize,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(submission);

            return Created("", new
            {
                submission.Id,
                submission.Status
            });
        }

        // GET /api/salaries/pending
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var submissions = await _repository.GetPendingAsync();
            return Ok(submissions);
        }

        // GET /api/salaries?createdAfter=2026-02-28T10:00:00Z
        [HttpGet]
        public async Task<IActionResult> GetPendingAfter(
            [FromQuery] DateTime? createdAfter)
        {
            if (createdAfter == null)
                return BadRequest("createdAfter query parameter is required");

            // Ensure UTC
            DateTime utcTime =
                createdAfter.Value.Kind == DateTimeKind.Utc
                ? createdAfter.Value
                : DateTime.SpecifyKind(createdAfter.Value, DateTimeKind.Utc);

            var submissions = await _repository.GetPendingAfterAsync(utcTime);
            return Ok(submissions);
        }
        
        // TODO Add company list API
        // TODO Add designation list API
        
        // TODO Add raw data api for stats ( approved submissions only )
        //  ( filterDto : role, country, company , experienceLevel )
        //  ( responseDto : SalarySubmission )
        
    }
}