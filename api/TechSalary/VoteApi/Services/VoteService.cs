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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private const string SalarySubmissionAPI = "SalarySubmissionApi";

        public VoteService(IVoteRepository repository, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<VoteResponse> CastVoteAsync(VoteRequest request)
        {
            var voteThreashold = _configuration.GetValue<int>("VotingParameters:VoteApprovalThreshold");
            var isApproved = false;
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

            if (total >= voteThreashold) 
            {
                await ApproveSubmissionAsync(request.SalarySubmissionId);
                isApproved = true;
            }

            return new VoteResponse
            {
                SalarySubmissionId = request.SalarySubmissionId,
                IsApproved = isApproved,
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

        private async Task<bool> ApproveSubmissionAsync(Guid submissionId)
        {
            var client = _httpClientFactory.CreateClient(SalarySubmissionAPI);

            var response = await client.PostAsync(
                $"/api/salaries/approve/{submissionId}",
                null
            );

            return response.IsSuccessStatusCode;
        }
    }
}
