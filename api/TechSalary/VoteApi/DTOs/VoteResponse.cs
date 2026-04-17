namespace VoteApi.DTOs
{
    public class VoteResponse
    {
        public Guid SalarySubmissionId { get; set; }
        public int TotalVotes { get; set; }
        public bool IsApproved { get; set; }
    }
}
