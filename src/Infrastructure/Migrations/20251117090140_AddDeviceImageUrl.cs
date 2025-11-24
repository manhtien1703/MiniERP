using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Devices",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 9, 1, 37, 914, DateTimeKind.Utc).AddTicks(8643), "$2a$11$Gp31/zzv.1EHegVnKCVubO7A8zyLQe1krFYDNhWdMbelf1RsFUg5S" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 9, 1, 38, 82, DateTimeKind.Utc).AddTicks(1006), "$2a$11$DSP005oYdXR/VOxEHbHdq.V9X5QUxin79kTY/8VZldCUVBqCvJpWS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 9, 1, 38, 230, DateTimeKind.Utc).AddTicks(303), "$2a$11$jfgS3Yt26RT.CxUe/PepOOx3C8HIPM/LreetXLQ5i4YOAo34JRFlO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Devices");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 16, 8, 6, 41, 662, DateTimeKind.Utc).AddTicks(2755), "$2a$11$msJFBe5lFbiL0TjKIM1IE.Zsa6REwc7H.pQHCmfBWfZNEpt4IvhZW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 16, 8, 6, 41, 815, DateTimeKind.Utc).AddTicks(6779), "$2a$11$TcyIkGxO3vGRZE52wzFbM.gn1LE38MfO7Tg.fUAA1JJtG6AJ.lOoW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 16, 8, 6, 41, 966, DateTimeKind.Utc).AddTicks(7618), "$2a$11$/PF5vrHiDfX1.ATSziaxH.MFVsdkvZy/G8XFoDiR4yQSnS75wYpTO" });
        }
    }
}
