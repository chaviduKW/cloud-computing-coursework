namespace VoteApi.DTOs
{
    public class VoteRequest
    {
        public Guid SalarySubmissionId { get; set; }
        public Guid UserId { get; set; }
        public string VoteType { get; set; } = string.Empty; // "UPVOTE" or "DOWNVOTE"
    }
}
