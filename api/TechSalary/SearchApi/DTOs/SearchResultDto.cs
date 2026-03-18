namespace SearchApi.DTOs
{
    public class SearchResultDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<SalaryRecordDto> Results { get; set; } = [];
    }

    public class SalaryRecordDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public decimal SalaryAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? Location { get; set; }
        public string? ExperienceLevel { get; set; }
        public int? YearsOfExperience { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int TotalVotes { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
