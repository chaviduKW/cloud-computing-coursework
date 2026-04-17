using VoteApi.Entities;

namespace VoteApi.Repositories
{
    public interface IVoteRepository
    {
        Task<Vote?> GetUserVoteAsync(Guid submissionId, Guid userId);
        Task<List<Vote>> GetVotesAsync(Guid? submissionId, Guid? userId);
        Task AddAsync(Vote vote);
        Task UpdateAsync(Vote vote);
        //Task<List<Vote>> GetVotesBySubmissionAsync(Guid submissionId);
        Task<int> GetTotalVotesBySubmissionIdAsync(Guid submissionId);
    }
}
