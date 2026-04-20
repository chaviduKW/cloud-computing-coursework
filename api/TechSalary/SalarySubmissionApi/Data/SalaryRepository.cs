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
                (id, country, company, role, "experienceLevel", "salaryAmount", currency, anonymize, status, "createdAt")
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
                ORDER BY "createdAt" DESC
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
                    experienceLevel,
                    salaryAmount,
                    currency,
                    anonymize,
                    status,
                    createdAt AS CreatedAt
                FROM salary.submissions
                WHERE status = 'PENDING'
                AND CreatedAt > @createdAfter
                ORDER BY CreatedAt
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

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
        {
            const string sql = """
                SELECT *
                FROM salary.companies
                ORDER BY companyName
            """;

            await _connection.OpenAsync();
            return await _connection.QueryAsync<Company>(sql);
        }

        public async Task<IEnumerable<Designation>> GetDesignationsAsync()
        {
            const string sql = """
                SELECT *
                FROM salary.designations
                ORDER BY designationName
            """;

            await _connection.OpenAsync();
            return await _connection.QueryAsync<Designation>(sql);
        }

        public async Task<IEnumerable<SalarySubmission>> GetApprovedAsync(SalaryFilterDto filter)
        {

            // Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            var conditions = new List<string> { "status = 'APPROVED'" };
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                conditions.Add("role ILIKE @Role");
                parameters.Add("Role", $"%{filter.Role}%");
            }
            if (!string.IsNullOrWhiteSpace(filter.Country))
            {
                conditions.Add("country ILIKE @Country");
                parameters.Add("Country", $"%{filter.Country}%");
            }
            if (!string.IsNullOrWhiteSpace(filter.Company))
            {
                conditions.Add("company ILIKE @Company");
                parameters.Add("Company", $"%{filter.Company}%");
            }
            if (!string.IsNullOrWhiteSpace(filter.ExperienceLevel))
            {
                conditions.Add("experience_level ILIKE @ExperienceLevel");
                parameters.Add("ExperienceLevel", $"%{filter.ExperienceLevel}%");
            }

            var where = string.Join(" AND ", conditions);

            var sql = $"""
                SELECT
                    id,
                    country AS Country,
                    company,
                    role,
                    "experienceLevel",
                    "salaryAmount",
                    currency,
                    anonymize,
                    status,
                    "createdAt"
                FROM salary.submissions
                WHERE {where}
                ORDER BY "createdAt" DESC
            """;

            await _connection.OpenAsync();

            var rawData = await _connection.QueryAsync<dynamic>(sql, parameters);


            return await _connection.QueryAsync<SalarySubmission>(sql, parameters);
        }

    }
}