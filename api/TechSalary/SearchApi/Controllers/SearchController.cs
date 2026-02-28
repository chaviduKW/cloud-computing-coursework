using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private static readonly ImmutableArray<SalaryRecord> SalaryRecords =
        [
            new SalaryRecord
            {
                SalaryAmount = 120000, CompanyName = "Google", Designation = "Software Engineer",
                SubmittedDate = new DateOnly(2025, 1, 10)
            },
            new SalaryRecord
            {
                SalaryAmount = 95000, CompanyName = "Microsoft", Designation = "Software Engineer",
                SubmittedDate = new DateOnly(2025, 2, 5)
            },
            new SalaryRecord
            {
                SalaryAmount = 150000, CompanyName = "Amazon", Designation = "Senior Engineer",
                SubmittedDate = new DateOnly(2025, 3, 12)
            },
            new SalaryRecord
            {
                SalaryAmount = 80000, CompanyName = "StartupX", Designation = "Backend Developer",
                SubmittedDate = new DateOnly(2024, 12, 20)
            },
            new SalaryRecord
            {
                SalaryAmount = 200000, CompanyName = "Google", Designation = "Staff Engineer",
                SubmittedDate = new DateOnly(2025, 4, 1)
            },
            new SalaryRecord
            {
                SalaryAmount = 110000, CompanyName = "Meta", Designation = "Software Engineer",
                SubmittedDate = new DateOnly(2025, 1, 25)
            },
            new SalaryRecord
            {
                SalaryAmount = 175000, CompanyName = "Netflix", Designation = "Senior Engineer",
                SubmittedDate = new DateOnly(2025, 5, 3)
            },
            new SalaryRecord
            {
                SalaryAmount = 90000, CompanyName = "StartupX", Designation = "Frontend Developer",
                SubmittedDate = new DateOnly(2025, 2, 14)
            },
            new SalaryRecord
            {
                SalaryAmount = 140000, CompanyName = "Microsoft", Designation = "Senior Engineer",
                SubmittedDate = new DateOnly(2025, 3, 30)
            },
            new SalaryRecord
            {
                SalaryAmount = 70000, CompanyName = "LocalCompany", Designation = "Junior Engineer",
                SubmittedDate = new DateOnly(2024, 11, 11)
            }
        ];

        [HttpGet]
        public IEnumerable<SalaryRecord> Get(
            [FromQuery] string? company,
            [FromQuery] string? designation,
            [FromQuery] DateOnly? submittedAfter,
            [FromQuery] DateOnly? submittedBefore)
        {
            var query = SalaryRecords.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(company))
            {
                query = query.Where(x =>
                    x.CompanyName.Contains(company, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(designation))
            {
                query = query.Where(x =>
                    x.Designation.Contains(designation, StringComparison.OrdinalIgnoreCase));
            }

            if (submittedAfter.HasValue)
            {
                query = query.Where(x =>
                    x.SubmittedDate >= submittedAfter.Value);
            }

            if (submittedBefore.HasValue)
            {
                query = query.Where(x =>
                    x.SubmittedDate <= submittedBefore.Value);
            }

            return query.ToList();
        }
    }
}