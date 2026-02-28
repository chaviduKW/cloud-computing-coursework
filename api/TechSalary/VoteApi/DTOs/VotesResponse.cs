namespace VoteApi.DTOs
{
    public class VotesResponse
    {
        public List<VoteDto> Votes { get; set; }
        public int TotalVotes { get; set; }
    }
}
