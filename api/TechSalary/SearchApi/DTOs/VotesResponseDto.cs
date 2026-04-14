namespace SearchApi.DTOs
{
    // Mirrors the shape returned by VoteApi GET /api/vote/{submissionId}
    public class VotesResponseDto
    {
        public List<VoteDtoItem> Votes { get; set; } = [];
        public int TotalVotes { get; set; }
    }

    public class VoteDtoItem
    {
        public Guid UserId { get; set; }
        public string VoteType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
