namespace SearchApi.DTOs
{
    // Mirrors the shape returned by SalarySubmissionApi GET /api/salaries/pending
    public class SalarySubmissionDto
    {
        public Guid Id { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public int SalaryAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool Anonymize { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
