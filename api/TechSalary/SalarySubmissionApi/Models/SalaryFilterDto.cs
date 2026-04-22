namespace SalarySubmissionApi.Models
{
    public class SalaryFilterDto
    {
        public string? Role { get; set; }
        public string? Country { get; set; }
        public string? Company { get; set; }
        public string? ExperienceLevel { get; set; }
    }
}