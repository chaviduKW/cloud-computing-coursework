using System.Runtime.InteropServices;
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

        [HttpPost("approve/{submissionId}")]
        public async Task<IActionResult> ApproveSubmission(
            [FromRoute] Guid submissionId)
        {
            if (submissionId == Guid.Empty)
                return BadRequest("Invalid submission ID");

            var result = await _repository.ApproveSubmissionAsync(submissionId);

            if (result)
            {
                return Ok();
            }

            return StatusCode(500, "Failed to approve submission");
        }


        // GET /api/salaries/companies
        // Returns distinct list of companies from approved submissions
        [HttpGet("companies")]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.GetCompaniesAsync();
            return Ok(companies);
        }


        // GET /api/salaries/designations
        // Returns distinct list of roles/designations from approved submissions
        [HttpGet("designations")]
        public async Task<IActionResult> GetDesignations()
        {
            var designations = await _repository.GetDesignationsAsync();
            return Ok(designations);
        }


        // GET /api/salaries/approved?role=...&country=...&company=...&experienceLevel=...
        // Returns approved submissions filtered by optional query parameters
        [HttpGet("approved")]
        public async Task<IActionResult> GetApproved([FromQuery] SalaryFilterDto filter)
        {

            var submissions = await _repository.GetApprovedAsync(filter);

            var result = submissions.Select(s => new
            {
                s.Id,
                s.Country,
                s.Company,
                s.Role,
                s.ExperienceLevel,
                s.SalaryAmount,
                s.Currency,
                s.Anonymize,
                s.Status,
                s.CreatedAt
            });

            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] SalaryFilterDto filter)
        {
            var submissions = await _repository.GetAllAsync(filter);

            var result = submissions.Select(s => new
            {
                s.Id,
                s.Country,
                s.Company,
                s.Role,
                s.ExperienceLevel,
                s.SalaryAmount,
                s.Currency,
                s.Anonymize,
                s.Status,
                s.CreatedAt
            });

            return Ok(result);


        }

    }
}