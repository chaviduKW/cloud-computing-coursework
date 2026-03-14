using VoteApi.DTOs;

namespace VoteApi.Services
{
    public interface IVoteService
    {
        Task<VoteResponse> CastVoteAsync(VoteRequest request);
        Task<VotesResponse> GetVotesAsync(Guid submissionId);
    }
}
