using Dapper;
using Npgsql;
using StatsApi.Models;

namespace StatsApi.Data;

public sealed class StatsRepository : IStatsRepository
{
    private readonly string _connectionString;

    public StatsRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<StatsResponse?> CalculateStatsAsync(
        string? role,
        string? country,
        string? company,
        string? experienceLevel)
    {
        // #region agent log
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(_connectionString);
            DebugLogger.Log(
                runId: "pre-fix",
                hypothesisId: "H1",
                location: "StatsRepository.CalculateStatsAsync:before-open",
                message: "Attempting DB connection",
                data: new { Host = builder.Host, Port = builder.Port, Database = builder.Database }
            );
        }
        catch
        {
            // ignore parsing issues; do not log secrets
        }
        // #endregion

        const string sql = """
            WITH filtered AS (
              SELECT salary_amount::numeric AS salary_amount
              FROM salary.submissions
              WHERE status = 'APPROVED'
                AND (@role IS NULL OR role ILIKE @role)
                AND (@country IS NULL OR country ILIKE @country)
                AND (@company IS NULL OR company ILIKE @company)
                AND (@experienceLevel IS NULL OR experience_level ILIKE @experienceLevel)
            )
            SELECT
              COUNT(*)::int AS RecordCount,
              COALESCE(AVG(salary_amount), 0)::numeric AS AverageSalary,
              COALESCE(PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY salary_amount), 0)::numeric AS MedianSalary,
              COALESCE(PERCENTILE_CONT(0.25) WITHIN GROUP (ORDER BY salary_amount), 0)::numeric AS P25Salary,
              COALESCE(PERCENTILE_CONT(0.75) WITHIN GROUP (ORDER BY salary_amount), 0)::numeric AS P75Salary
            FROM filtered;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);

        try
        {
            var row = await connection.QuerySingleAsync<StatsRow>(sql, new
            {
                role,
                country,
                company,
                experienceLevel
            });

            // #region agent log
            DebugLogger.Log(
                runId: "pre-fix",
                hypothesisId: "H2",
                location: "StatsRepository.CalculateStatsAsync:after-query",
                message: "Query executed",
                data: new { row.RecordCount });
            // #endregion

            if (row.RecordCount <= 0)
                return null;

            return new StatsResponse
            {
                Role = role ?? "ALL",
                Country = country ?? "ALL",
                Company = company ?? "ALL",
                ExperienceLevel = experienceLevel ?? "ALL",
                RecordCount = row.RecordCount,
                AverageSalary = row.AverageSalary,
                MedianSalary = row.MedianSalary,
                P25Salary = row.P25Salary,
                P75Salary = row.P75Salary,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            // #region agent log
            DebugLogger.Log(
                runId: "pre-fix",
                hypothesisId: "H1",
                location: "StatsRepository.CalculateStatsAsync:exception",
                message: "DB query failed",
                data: new { ex.GetType().FullName, ex.Message });
            // #endregion
            throw;
        }
    }

    private sealed class StatsRow
    {
        public int RecordCount { get; init; }
        public decimal AverageSalary { get; init; }
        public decimal MedianSalary { get; init; }
        public decimal P25Salary { get; init; }
        public decimal P75Salary { get; init; }
    }
}

