using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoteApi.DTOs;
using VoteApi.Services;

namespace VoteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _voteService;

        public VoteController(IVoteService voteService)
        {
            _voteService = voteService;
        }

        // POST: api/votes
        [HttpPost]
        public async Task<IActionResult> CastVote([FromBody] VoteRequest request)
        {

            var result = await _voteService.CastVoteAsync(request);

            return Ok(result);
        }

        // GET: api/votes/{submissionId}
        [HttpGet("{submissionId}")]
        public async Task<IActionResult> GetVotes(Guid submissionId)
        {
            var result = await _voteService.GetVotesAsync(submissionId);

            return Ok(result);
        }
    }
}
