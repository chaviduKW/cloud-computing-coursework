using Dapper;
using Npgsql;
using SalarySubmissionApi.Models;

namespace SalarySubmissionApi.Data
{
    public class SalaryRepository
    {
        private readonly string _connectionString;

        public SalaryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Database connection string is not configured.");
        }

        public async Task CreateAsync(SalarySubmission submission)
        {
            const string sql = """
                INSERT INTO salary.submissions
                (id, country, company, role, experiencelevel, salaryamount, currency, anonymize, status, createdat)
                VALUES
                (@Id, @Country, @Company, @Role, @ExperienceLevel, @SalaryAmount, @Currency, @Anonymize, @Status, @CreatedAt)
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, submission);
        }

        public async Task<IEnumerable<SalarySubmission>> GetPendingAsync()
        {
            const string sql = """
                SELECT
                    id,
                    country,
                    company,
                    role,
                    experiencelevel AS ExperienceLevel,
                    salaryamount AS SalaryAmount,
                    currency,
                    anonymize,
                    status,
                    createdat AS CreatedAt
                FROM salary.submissions
                WHERE status = 'PENDING'
                ORDER BY createdat DESC
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<SalarySubmission>(sql);
        }

        public async Task<IEnumerable<SalarySubmission>> GetPendingAfterAsync(DateTime createdAfter)
        {
            const string sql = """
                SELECT
                    id,
                    country,
                    company,
                    role,
                    experiencelevel AS ExperienceLevel,
                    salaryamount AS SalaryAmount,
                    currency,
                    anonymize,
                    status,
                    createdat AS CreatedAt
                FROM salary.submissions
                WHERE status = 'PENDING'
                AND createdat > @CreatedAfter
                ORDER BY createdat
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<SalarySubmission>(sql, new { CreatedAfter = createdAfter });
        }

        public async Task<bool> ApproveSubmissionAsync(Guid submissionId)
        {
            const string sql = """
                UPDATE salary.submissions
                SET status = 'APPROVED'
                WHERE id = @SubmissionId
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            var rowsAffected = await connection.ExecuteAsync(sql, new { SubmissionId = submissionId });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<string>> GetCompaniesAsync()
        {
            const string sql = """
                SELECT companyname
                FROM salary.companies
                ORDER BY companyname
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<string>(sql);
        }

        public async Task<IEnumerable<string>> GetDesignationsAsync()
        {
            const string sql = """
                SELECT designationname
                FROM salary.designations
                ORDER BY designationname
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<string>(sql);
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
                conditions.Add("experiencelevel = @ExperienceLevel");
                parameters.Add("ExperienceLevel", filter.ExperienceLevel);
            }

            var where = string.Join(" AND ", conditions);

            var sql = $"""
                SELECT
                    id,
                    country,
                    company,
                    role,
                    experiencelevel AS ExperienceLevel,
                    salaryamount AS SalaryAmount,
                    currency,
                    anonymize,
                    status,
                    createdat AS CreatedAt
                FROM salary.submissions
                WHERE {where}
                ORDER BY createdat DESC
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<SalarySubmission>(sql, parameters);
        }
    }
}