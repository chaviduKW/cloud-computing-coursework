using Microsoft.EntityFrameworkCore;
using VoteApi.Data;
using VoteApi.Entities;

namespace VoteApi.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly VoteDbContext _context;

        public VoteRepository(VoteDbContext context)
        {
            _context = context;
        }

        public async Task<Vote?> GetUserVoteAsync(Guid submissionId, Guid userId)
        {
            return await _context.Votes
                .FirstOrDefaultAsync(v =>
                    v.SalarySubmissionId == submissionId &&
                    v.UserId == userId);
        }

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vote vote)
        {
            _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Vote>> GetVotesBySubmissionAsync(Guid submissionId)
        {
            return await _context.Votes
                .Where(v => v.SalarySubmissionId == submissionId)
                .ToListAsync();
        }

        public async Task<int> GetTotalVotesAsync(Guid submissionId)
        {
            return await _context.Votes
                .Where(v => v.SalarySubmissionId == submissionId)
                .SumAsync(v => (int)v.VoteType);
        }
    }
}
