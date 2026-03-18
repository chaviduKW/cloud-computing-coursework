namespace StatsApi.Models
{
    public class StatsResponse
    {
        public string Role { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public decimal AverageSalary { get; set; }
        public decimal MedianSalary { get; set; }
        public decimal P25Salary { get; set; }
        public decimal P75Salary { get; set; }
        public int RecordCount { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}