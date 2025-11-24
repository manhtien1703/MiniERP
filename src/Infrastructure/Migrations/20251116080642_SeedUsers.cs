using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 16, 8, 6, 41, 662, DateTimeKind.Utc).AddTicks(2755), "admin@minierp.com", "Administrator", true, null, "$2a$11$msJFBe5lFbiL0TjKIM1IE.Zsa6REwc7H.pQHCmfBWfZNEpt4IvhZW", "admin" },
                    { 2, new DateTime(2025, 11, 16, 8, 6, 41, 815, DateTimeKind.Utc).AddTicks(6779), "user1@minierp.com", "Nguyễn Văn A", true, null, "$2a$11$TcyIkGxO3vGRZE52wzFbM.gn1LE38MfO7Tg.fUAA1JJtG6AJ.lOoW", "user1" },
                    { 3, new DateTime(2025, 11, 16, 8, 6, 41, 966, DateTimeKind.Utc).AddTicks(7618), "user2@minierp.com", "Trần Thị B", true, null, "$2a$11$/PF5vrHiDfX1.ATSziaxH.MFVsdkvZy/G8XFoDiR4yQSnS75wYpTO", "user2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
