using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VoteApi.Models.Enums;

namespace VoteApi.Entities
{
    [Table("votes", Schema = "community")]
    public class Vote
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("salarysubmissionid")]
        public Guid SalarySubmissionId { get; set; }

        [Column("userid")]
        public Guid UserId { get; set; }

        [Column("votetype")]
        public VoteType VoteType { get; set; }

        [Column("createdat")]
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