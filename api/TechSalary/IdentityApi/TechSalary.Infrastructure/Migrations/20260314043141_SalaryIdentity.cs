using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSalaryIdentity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalaryIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C001",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 31, 41, 546, DateTimeKind.Utc).AddTicks(7859));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C002",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 31, 41, 546, DateTimeKind.Utc).AddTicks(7860));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C003",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 31, 41, 546, DateTimeKind.Utc).AddTicks(7861));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C001",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 30, 8, 106, DateTimeKind.Utc).AddTicks(2741));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C002",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 30, 8, 106, DateTimeKind.Utc).AddTicks(2743));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerCode",
                keyValue: "C003",
                column: "CreatedAt",
                value: new DateTime(2026, 3, 14, 4, 30, 8, 106, DateTimeKind.Utc).AddTicks(2744));
        }
    }
}
