using Microsoft.EntityFrameworkCore;
using SearchApi.Data;
using SearchApi.DTOs;
using SearchApi.Entities;

namespace SearchApi.Services
{
    public class SearchService(SearchDbContext db) : ISearchService
    {
        public async Task<SearchResultDto> SearchAsync(SearchQueryDto query, CancellationToken cancellationToken = default)
        {
            var q = db.SalaryRecords.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Company))
                q = q.Where(r => EF.Functions.ILike(r.CompanyName, $"%{query.Company}%"));

            if (!string.IsNullOrWhiteSpace(query.Designation))
                q = q.Where(r => EF.Functions.ILike(r.Designation, $"%{query.Designation}%"));

            if (!string.IsNullOrWhiteSpace(query.Location))
                q = q.Where(r => r.Location != null && EF.Functions.ILike(r.Location, $"%{query.Location}%"));

            if (!string.IsNullOrWhiteSpace(query.ExperienceLevel))
                q = q.Where(r => r.ExperienceLevel == query.ExperienceLevel);

            if (query.MinSalary.HasValue)
                q = q.Where(r => r.SalaryAmount >= query.MinSalary.Value);

            if (query.MaxSalary.HasValue)
                q = q.Where(r => r.SalaryAmount <= query.MaxSalary.Value);

            if (query.SubmittedAfter.HasValue)
                q = q.Where(r => r.SubmittedAt >= query.SubmittedAfter.Value);

            if (query.SubmittedBefore.HasValue)
                q = q.Where(r => r.SubmittedAt <= query.SubmittedBefore.Value);

            var totalCount = await q.CountAsync(cancellationToken);

            q = (query.SortBy.ToLower(), query.SortOrder.ToLower()) switch
            {
                ("salary", "asc")  => q.OrderBy(r => r.SalaryAmount),
                ("salary", _)      => q.OrderByDescending(r => r.SalaryAmount),
                ("votes", "asc")   => q.OrderBy(r => r.TotalVotes),
                ("votes", _)       => q.OrderByDescending(r => r.TotalVotes),
                ("date", "asc")    => q.OrderBy(r => r.SubmittedAt),
                _                  => q.OrderByDescending(r => r.SubmittedAt),
            };

            var pageSize = Math.Clamp(query.PageSize, 1, 100);
            var page = Math.Max(1, query.Page);

            var results = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new SalaryRecordDto
                {
                    Id = r.Id,
                    CompanyName = r.CompanyName,
                    Designation = r.Designation,
                    SalaryAmount = r.SalaryAmount,
                    Currency = r.Currency,
                    Location = r.Location,
                    ExperienceLevel = r.ExperienceLevel,
                    YearsOfExperience = r.YearsOfExperience,
                    UpVotes = r.UpVotes,
                    DownVotes = r.DownVotes,
                    TotalVotes = r.TotalVotes,
                    SubmittedAt = r.SubmittedAt,
                })
                .ToListAsync(cancellationToken);

            return new SearchResultDto
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Results = results,
            };
        }

        public async Task<IEnumerable<string>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            return await db.SalaryRecords
                .AsNoTracking()
                .Select(r => r.CompanyName)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetDesignationsAsync(CancellationToken cancellationToken = default)
        {
            return await db.SalaryRecords
                .AsNoTracking()
                .Select(r => r.Designation)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync(cancellationToken);
        }
    }
}
