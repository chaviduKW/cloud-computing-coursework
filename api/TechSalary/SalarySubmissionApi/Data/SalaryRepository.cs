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
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task CreateAsync(SalarySubmission submission)
        {
            const string sql = """
                INSERT INTO salary.submissions
                (id, country, company, role, experience_level, salary_amount, currency, anonymize, status, created_at)
                VALUES
                (@Id, @Country, @Company, @Role, @ExperienceLevel, @SalaryAmount, @Currency, @Anonymize, @Status, @CreatedAt)
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, submission);
        }
 
        public async Task<IEnumerable<SalarySubmission>> GetPendingAsync()
        {
            const string sql = """
                SELECT *
                FROM salary.submissions
                WHERE status = 'PENDING'
                ORDER BY created_at DESC
            """;

            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<SalarySubmission>(sql);
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

            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<SalarySubmission>(sql, new
            {
                CreatedAfter = createdAfter
            });
        }
    }
}