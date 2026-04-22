namespace VoteApi.DTOs
{
    public class VotesResponse
    {
        public List<VoteDto> Votes { get; set; }
        public int TotalVotes { get; set; }
        public int UpVoteCount { get; set; }
        public int DownVoteCount { get; set; }
    }
}
