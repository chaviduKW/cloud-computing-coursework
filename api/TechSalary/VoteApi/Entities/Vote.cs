using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VoteApi.Models.Enums;

namespace VoteApi.Entities
{
    public class Vote
    {
        public Guid Id { get; set; }
        public Guid SalarySubmissionId { get; set; }
        public Guid UserId { get; set; }
        public VoteType VoteType { get; set; }
        public DateTime CreatedAt { get; set; }

        private Vote() { } // EF Core requires a parameterless constructor

        public Vote(Guid salarySubmissionId, Guid userId, VoteType voteType)
        {
            Id = Guid.NewGuid();
            SalarySubmissionId = salarySubmissionId;
            UserId = userId;
            VoteType = voteType;
            CreatedAt = DateTime.UtcNow;
        }

        public void ChangeVote(VoteType voteType)
        {
            VoteType = voteType;
        }
    }
}
