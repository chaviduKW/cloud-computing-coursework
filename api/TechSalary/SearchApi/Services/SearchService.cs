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

            var pageItems = sorted
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Fetch vote counts from VoteApi in parallel for this page
            var voteClient = httpClientFactory.CreateClient("VoteApi");
            var voteTasks = pageItems.Select(item => FetchVotesAsync(voteClient, item.Id, cancellationToken));
            var voteResults = await Task.WhenAll(voteTasks);
            var votesById = voteResults.ToDictionary(v => v.Id);

            var results = pageItems.Select(r =>
            {
                var votes = votesById.GetValueOrDefault(r.Id);
                return new SalaryRecordDto
                {
                    Id = r.Id,
                    Company = r.Company,
                    Role = r.Role,
                    SalaryAmount = r.SalaryAmount,
                    Currency = r.Currency,
                    Country = r.Country,
                    ExperienceLevel = r.ExperienceLevel,
                    UpVotes = votes.UpVotes,
                    DownVotes = votes.DownVotes,
                    TotalVotes = votes.TotalVotes,
                    CreatedAt = r.CreatedAt,
                };
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

        public async Task<IEnumerable<string>> GetCompaniesAsync(CancellationToken cancellationToken = default)
        {
            var submissions = await FetchSubmissionsAsync(cancellationToken);
            return submissions.Select(r => r.Company).Distinct().OrderBy(c => c);
        }

        public async Task<IEnumerable<string>> GetDesignationsAsync(CancellationToken cancellationToken = default)
        {
            var submissions = await FetchSubmissionsAsync(cancellationToken);
            return submissions.Select(r => r.Role).Distinct().OrderBy(d => d);
        }

        private async Task<List<SalarySubmissionDto>> FetchSubmissionsAsync(CancellationToken cancellationToken)
        {
            var client = httpClientFactory.CreateClient("SalarySubmissionApi");
            return await client.GetFromJsonAsync<List<SalarySubmissionDto>>("/api/salaries/pending", cancellationToken)
                ?? [];
        }

        private async Task<(Guid Id, int UpVotes, int DownVotes, int TotalVotes)> FetchVotesAsync(
            HttpClient client, Guid submissionId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await client.GetFromJsonAsync<DTOs.VotesResponseDto>(
                    $"/api/vote/{submissionId}", cancellationToken);

                if (response is null)
                    return (submissionId, 0, 0, 0);

                var upVotes = response.Votes.Count(v => v.VoteType.Equals("UP", StringComparison.OrdinalIgnoreCase));
                var downVotes = response.Votes.Count(v => v.VoteType.Equals("DOWN", StringComparison.OrdinalIgnoreCase));
                return (submissionId, upVotes, downVotes, response.TotalVotes);
            }
            catch
            {
                return (submissionId, 0, 0, 0);
            }
        }
    }
}
