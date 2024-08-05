using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventModeColumnAddedToEventTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventMode",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 5, 2, 37, 53, 368, DateTimeKind.Utc).AddTicks(4968));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 5, 2, 37, 53, 368, DateTimeKind.Utc).AddTicks(4975));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 5, 2, 37, 53, 368, DateTimeKind.Utc).AddTicks(6187));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventMode",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 7, 20, 11, 10, 12, 833, DateTimeKind.Utc).AddTicks(4335));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 7, 20, 11, 10, 12, 833, DateTimeKind.Utc).AddTicks(4341));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 7, 20, 11, 10, 12, 833, DateTimeKind.Utc).AddTicks(5618));
        }
    }
}
