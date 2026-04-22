using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteApi.Migrations
{
    /// <inheritdoc />
    public partial class communityschemanew : Migration
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    salarysubmissionid = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    votetype = table.Column<int>(type: "integer", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_votes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_votes_salarysubmissionid_userid",
                schema: "community",
                table: "votes",
                columns: new[] { "salarysubmissionid", "userid" },
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
