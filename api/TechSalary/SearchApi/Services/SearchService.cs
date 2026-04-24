using SearchApi.DTOs;
using System.Net.Http.Json;

namespace SearchApi.Services
{
    public class SearchService(IHttpClientFactory httpClientFactory) : ISearchService
    {
        public async Task<SearchResultDto> SearchAsync(SearchQueryDto query, CancellationToken cancellationToken = default)
        {
            var submissions = await FetchSubmissionsAsync(cancellationToken);

            IEnumerable<SalarySubmissionDto> q = submissions;

            if (!string.IsNullOrWhiteSpace(query.Company))
                q = q.Where(r => r.Company.Contains(query.Company, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Designation))
                q = q.Where(r => r.Role.Contains(query.Designation, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Location))
                q = q.Where(r => r.Country.Contains(query.Location, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.ExperienceLevel))
                q = q.Where(r => r.ExperienceLevel.Equals(query.ExperienceLevel, StringComparison.OrdinalIgnoreCase));

            if (query.MinSalary.HasValue)
                q = q.Where(r => r.SalaryAmount >= query.MinSalary.Value);

            if (query.MaxSalary.HasValue)
                q = q.Where(r => r.SalaryAmount <= query.MaxSalary.Value);

            if (query.SubmittedAfter.HasValue)
                q = q.Where(r => r.CreatedAt >= query.SubmittedAfter.Value);

            if (query.SubmittedBefore.HasValue)
                q = q.Where(r => r.CreatedAt <= query.SubmittedBefore.Value);

            var filtered = q.ToList();
            var totalCount = filtered.Count;

            IEnumerable<SalarySubmissionDto> sorted = (query.SortBy.ToLower(), query.SortOrder.ToLower()) switch
            {
                ("salary", "asc") => filtered.OrderBy(r => r.SalaryAmount),
                ("salary", _)     => filtered.OrderByDescending(r => r.SalaryAmount),
                ("date", "asc")   => filtered.OrderBy(r => r.CreatedAt),
                _                 => filtered.OrderByDescending(r => r.CreatedAt),
            };

            var pageSize = Math.Clamp(query.PageSize, 1, 100);
            var page = Math.Max(1, query.Page);

            var results = sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new SalaryRecordDto
                {
                    Id = r.Id,
                    Company = r.Company,
                    Role = r.Role,
                    SalaryAmount = r.SalaryAmount,
                    Currency = r.Currency,
                    Country = r.Country,
                    ExperienceLevel = r.ExperienceLevel,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status
                }).ToList();

            return new SearchResultDto
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Results = results,
            };
        }

        private async Task<List<SalarySubmissionDto>> FetchSubmissionsAsync(CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient("SalarySubmissionApi");
            return await client.GetFromJsonAsync<List<SalarySubmissionDto>>("/api/salaries/all", cancellationToken)
                ?? [];
        }

    }
}
