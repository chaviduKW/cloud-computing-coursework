namespace SearchApi.DTOs
{
    public class SearchQueryDto
    {
        public string? Company { get; set; }
        public string? Designation { get; set; }
        public string? Location { get; set; }
        public string? ExperienceLevel { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public DateTime? SubmittedAfter { get; set; }
        public DateTime? SubmittedBefore { get; set; }

        /// <summary>salary | votes | date</summary>
        public string SortBy { get; set; } = "date";

        /// <summary>asc | desc</summary>
        public string SortOrder { get; set; } = "desc";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
