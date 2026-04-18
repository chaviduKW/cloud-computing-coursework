using Dapper;
using Npgsql;
using SalarySubmissionApi.Models;

namespace SalarySubmissionApi.Data
{
    public class SalaryRepository
    {
        private readonly NpgsqlConnection _connection;

        public SalaryRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public async Task CreateAsync(SalarySubmission submission)
        {
            const string sql = """
                INSERT INTO salary.submissions
                (id, country, company, role, experience_level, salary_amount, currency, anonymize, status, created_at)
                VALUES
                (@Id, @Country, @Company, @Role, @ExperienceLevel, @SalaryAmount, @Currency, @Anonymize, @Status, @CreatedAt)
            """;

            await _connection.OpenAsync();
            await _connection.ExecuteAsync(sql, submission);
        }
 
        public async Task<IEnumerable<SalarySubmission>> GetPendingAsync()
        {
            const string sql = """
                SELECT *
                FROM salary.submissions
                WHERE status = 'PENDING'
                ORDER BY created_at DESC
            """;

            await _connection.OpenAsync();
            return await _connection.QueryAsync<SalarySubmission>(sql);
        }

        //get all PENDING submissions created after a given time
        public async Task<IEnumerable<SalarySubmission>> GetPendingAfterAsync(DateTime createdAfter)
        {
            const string sql = """
                SELECT
                    id,
                    country,
                    company,
                    role,
                    experience_level AS ExperienceLevel,
                    salary_amount AS SalaryAmount,
                    currency,
                    anonymize,
                    status,
                    created_at AS CreatedAt
                FROM salary.submissions
                WHERE status = 'PENDING'
                AND created_at > @createdAfter
                ORDER BY created_at
            """;

            await _connection.OpenAsync();
            return await _connection.QueryAsync<SalarySubmission>(sql, new
            {
                CreatedAfter = createdAfter
            });
        }

        public async Task<bool> ApproveSubmissionAsync(Guid submissionId)
        {
            const string sql = """
                UPDATE salary.submissions
                SET status = 'APPROVED'
                WHERE id = @SubmissionId
            """;

            await _connection.OpenAsync();

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                SubmissionId = submissionId
            });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<string>> GetCompaniesAsync()
{
    const string sql = """
        SELECT DISTINCT company
        FROM salary.submissions
        WHERE status = 'APPROVED'
        ORDER BY company
    """;

    await _connection.OpenAsync();
    return await _connection.QueryAsync<string>(sql);
}

public async Task<IEnumerable<string>> GetDesignationsAsync()
{
    const string sql = """
        SELECT DISTINCT role
        FROM salary.submissions
        WHERE status = 'APPROVED'
        ORDER BY role
    """;

    await _connection.OpenAsync();
    return await _connection.QueryAsync<string>(sql);
}

public async Task<IEnumerable<SalarySubmission>> GetApprovedAsync(SalaryFilterDto filter)
{
    var conditions = new List<string> { "status = 'APPROVED'" };
    var parameters = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(filter.Role))
    {
        conditions.Add("role = @Role");
        parameters.Add("Role", filter.Role);
    }
    if (!string.IsNullOrWhiteSpace(filter.Country))
    {
        conditions.Add("country = @Country");
        parameters.Add("Country", filter.Country);
    }
    if (!string.IsNullOrWhiteSpace(filter.Company))
    {
        conditions.Add("company = @Company");
        parameters.Add("Company", filter.Company);
    }
    if (!string.IsNullOrWhiteSpace(filter.ExperienceLevel))
    {
        conditions.Add("experience_level = @ExperienceLevel");
        parameters.Add("ExperienceLevel", filter.ExperienceLevel);
    }

    var where = string.Join(" AND ", conditions);

    var sql = $"""
        SELECT
            id,
            country,
            company,
            role,
            experience_level AS ExperienceLevel,
            salary_amount    AS SalaryAmount,
            currency,
            anonymize,
            status,
            created_at       AS CreatedAt
        FROM salary.submissions
        WHERE {where}
        ORDER BY created_at DESC
    """;

    await _connection.OpenAsync();
    return await _connection.QueryAsync<SalarySubmission>(sql, parameters);
        }

    }
}