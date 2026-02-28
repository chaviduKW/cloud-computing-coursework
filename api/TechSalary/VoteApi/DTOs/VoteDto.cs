namespace VoteApi.DTOs
{
    public class VoteDto
    {
        public Guid UserId { get; set; }
        public string VoteType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
