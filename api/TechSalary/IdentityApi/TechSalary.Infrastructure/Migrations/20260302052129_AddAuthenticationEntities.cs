using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSalaryIdentity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomerCode",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerCode",
                table: "Customers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(450)");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "User"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AuthRefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_AuthRefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C001",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 5, 21, 29, 573, DateTimeKind.Utc).AddTicks(2892));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C002",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 5, 21, 29, 573, DateTimeKind.Utc).AddTicks(2893));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C003",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 2, 5, 21, 29, 573, DateTimeKind.Utc).AddTicks(2894));

            migrationBuilder.CreateIndex(
                name: "IX_AuthRefreshTokens_UserId",
                table: "AuthRefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthRefreshTokens");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerCode",
                table: "Orders",
                type: "character varying(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerCode",
                table: "Customers",
                type: "character varying(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C001",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 18, 42, 53, 466, DateTimeKind.Utc).AddTicks(170));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C002",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 18, 42, 53, 466, DateTimeKind.Utc).AddTicks(171));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C003",
                column: "CreatedAt",
                value: new DateTime(2026, 2, 25, 18, 42, 53, 466, DateTimeKind.Utc).AddTicks(172));
        }
    }
}
