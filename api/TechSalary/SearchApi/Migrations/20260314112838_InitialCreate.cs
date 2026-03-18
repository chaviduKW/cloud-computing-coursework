using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "salary_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Designation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SalaryAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ExperienceLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: true),
                    UpVotes = table.Column<int>(type: "integer", nullable: false),
                    DownVotes = table.Column<int>(type: "integer", nullable: false),
                    TotalVotes = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_records", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_CompanyName",
                table: "salary_records",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_CompanyName_Designation",
                table: "salary_records",
                columns: new[] { "CompanyName", "Designation" });

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_Designation",
                table: "salary_records",
                column: "Designation");

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_ExperienceLevel",
                table: "salary_records",
                column: "ExperienceLevel");

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_Location",
                table: "salary_records",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_SalaryAmount",
                table: "salary_records",
                column: "SalaryAmount");

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_SubmittedAt",
                table: "salary_records",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_salary_records_TotalVotes",
                table: "salary_records",
                column: "TotalVotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "salary_records");
        }
    }
}
