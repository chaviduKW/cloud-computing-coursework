using VoteApi.DTOs;
using VoteApi.Entities;
using VoteApi.Models;
using VoteApi.Models.Enums;
using VoteApi.Repositories;

namespace VoteApi.Services
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _repository;

        public VoteService(IVoteRepository repository)
        {
            _repository = repository;
        }

        public async Task<VoteResponse> CastVoteAsync(VoteRequest request)
        {
            var voteType = Enum.Parse<VoteType>(request.VoteType, true);

            var existingVote = await _repository
                .GetUserVoteAsync(request.SalarySubmissionId, request.UserId);

            if (existingVote == null)
            {
                var vote = new Vote(
                    request.SalarySubmissionId,
                    request.UserId,
                    voteType
                );

                await _repository.AddAsync(vote);
            }
            else
            {
                existingVote.ChangeVote(voteType);
                await _repository.UpdateAsync(existingVote);
            }

            var total = await _repository
                .GetTotalVotesBySubmissionIdAsync(request.SalarySubmissionId);

            return new VoteResponse
            {
                SalarySubmissionId = request.SalarySubmissionId,
                TotalVotes = total
            };
        }

        public async Task<VotesResponse> GetVotesAsync(Guid? submissionId, Guid? userId)
        {
            var votes = await _repository
                .GetVotesAsync(submissionId,userId);

            var votesDto =  votes.Select(v => new VoteDto
            {
                UserId = v.UserId,
                SubmissionId = v.SalarySubmissionId,
                VoteType = v.VoteType.ToString(),
                CreatedAt = v.CreatedAt
            }).ToList();

            var votesResponse = new VotesResponse()
            {
                Votes = votesDto,
                TotalVotes = votes.Sum(v => (int)v.VoteType)
            };

            return votesResponse;


        }
    }
}
