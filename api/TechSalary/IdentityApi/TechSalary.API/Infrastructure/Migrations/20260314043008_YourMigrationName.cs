using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class YourMigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
