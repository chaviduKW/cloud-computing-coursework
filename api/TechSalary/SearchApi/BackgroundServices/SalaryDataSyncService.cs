using Microsoft.EntityFrameworkCore;
using SearchApi.Data;
using SearchApi.DTOs;
using SearchApi.Entities;

namespace SearchApi.BackgroundServices
{
    public class SalaryDataSyncService(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<SalaryDataSyncService> logger) : BackgroundService
    {
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(
            config.GetValue<int>("SyncSettings:IntervalMinutes", 60));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("SalaryDataSyncService started. Sync interval: {Interval}", _interval);

            // Run an initial sync immediately on startup
            await SyncAsync(stoppingToken);

            using var timer = new PeriodicTimer(_interval);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await SyncAsync(stoppingToken);
            }
        }

        private async Task SyncAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting salary data sync at {Time}", DateTime.UtcNow);

            try
            {
                var submissions = await FetchSubmissionsAsync(cancellationToken);
                if (submissions is null)
                {
                    logger.LogWarning("Sync skipped: could not reach SalarySubmissionApi.");
                    return;
                }

                await UpsertRecordsAsync(submissions, cancellationToken);
                logger.LogInformation("Sync complete. {Count} records processed.", submissions.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error during salary data sync.");
            }
        }

        private async Task<List<SalarySubmissionDto>?> FetchSubmissionsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var client = httpClientFactory.CreateClient("SalarySubmissionApi");
                var response = await client.GetAsync("/api/submissions", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("SalarySubmissionApi returned {StatusCode}", response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<List<SalarySubmissionDto>>(cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Could not connect to SalarySubmissionApi.");
                return null;
            }
        }

        private async Task UpsertRecordsAsync(List<SalarySubmissionDto> submissions, CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SearchDbContext>();

            var incomingIds = submissions.Select(s => s.Id).ToHashSet();
            var existingIds = await db.SalaryRecords
                .Where(r => incomingIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToHashSetAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var toInsert = new List<SalaryRecord>();

            foreach (var submission in submissions)
            {
                if (existingIds.Contains(submission.Id))
                {
                    // Update existing record
                    await db.SalaryRecords
                        .Where(r => r.Id == submission.Id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(r => r.CompanyName, submission.CompanyName)
                            .SetProperty(r => r.Designation, submission.Designation)
                            .SetProperty(r => r.SalaryAmount, submission.SalaryAmount)
                            .SetProperty(r => r.Currency, submission.Currency)
                            .SetProperty(r => r.Location, submission.Location)
                            .SetProperty(r => r.ExperienceLevel, submission.ExperienceLevel)
                            .SetProperty(r => r.YearsOfExperience, submission.YearsOfExperience)
                            .SetProperty(r => r.UpVotes, submission.UpVotes)
                            .SetProperty(r => r.DownVotes, submission.DownVotes)
                            .SetProperty(r => r.TotalVotes, submission.TotalVotes)
                            .SetProperty(r => r.SubmittedAt, submission.SubmittedAt)
                            .SetProperty(r => r.LastSyncedAt, now),
                        cancellationToken);
                }
                else
                {
                    toInsert.Add(new SalaryRecord
                    {
                        Id = submission.Id,
                        CompanyName = submission.CompanyName,
                        Designation = submission.Designation,
                        SalaryAmount = submission.SalaryAmount,
                        Currency = submission.Currency,
                        Location = submission.Location,
                        ExperienceLevel = submission.ExperienceLevel,
                        YearsOfExperience = submission.YearsOfExperience,
                        UpVotes = submission.UpVotes,
                        DownVotes = submission.DownVotes,
                        TotalVotes = submission.TotalVotes,
                        SubmittedAt = submission.SubmittedAt,
                        LastSyncedAt = now,
                    });
                }
            }

            if (toInsert.Count > 0)
            {
                db.SalaryRecords.AddRange(toInsert);
                await db.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Inserted {Count} new records.", toInsert.Count);
            }
        }
    }
}
