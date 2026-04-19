using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteApi.Migrations
{
    /// <inheritdoc />
    public partial class initialvoteApimigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "community");

            migrationBuilder.CreateTable(
                name: "votes",
                schema: "community",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SalarySubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VoteType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_votes_SalarySubmissionId_UserId",
                schema: "community",
                table: "votes",
                columns: new[] { "SalarySubmissionId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "votes",
                schema: "community");
        }
    }
}
