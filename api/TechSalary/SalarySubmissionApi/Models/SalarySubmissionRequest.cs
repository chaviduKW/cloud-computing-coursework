namespace SalarySubmissionApi.Models
{
    public class SalarySubmissionRequest
    {
        public string Country { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public long SalaryAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool Anonymize { get; set; }
    }
}