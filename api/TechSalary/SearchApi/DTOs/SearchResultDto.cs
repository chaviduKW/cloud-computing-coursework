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
        public string Company { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int SalaryAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int TotalVotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
