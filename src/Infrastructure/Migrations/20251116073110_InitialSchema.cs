using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouses_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    WarehouseId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeviceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Temperature = table.Column<double>(type: "double", nullable: false),
                    Humidity = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceLogs_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Provinces",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "TQ", "Tuyên Quang" },
                    { 2, "LC", "Lào Cai" },
                    { 3, "TNG", "Thái Nguyên" },
                    { 4, "PT", "Phú Thọ" },
                    { 5, "BN", "Bắc Ninh" },
                    { 6, "HY", "Hưng Yên" },
                    { 7, "HP", "Hải Phòng" },
                    { 8, "NB", "Ninh Bình" },
                    { 9, "QT", "Quảng Trị" },
                    { 10, "DAN", "Đà Nẵng" },
                    { 11, "QNG", "Quảng Ngãi" },
                    { 12, "GL", "Gia Lai" },
                    { 13, "KH", "Khánh Hòa" },
                    { 14, "LD", "Lâm Đồng" },
                    { 15, "DLK", "Đắk Lắk" },
                    { 16, "HCM", "Hồ Chí Minh" },
                    { 17, "DNI", "Đồng Nai" },
                    { 18, "TNI", "Tây Ninh" },
                    { 19, "CT", "Cần Thơ" },
                    { 20, "VL", "Vĩnh Long" },
                    { 21, "DTP", "Đồng Tháp" },
                    { 22, "CM", "Cà Mau" },
                    { 23, "AG", "An Giang" },
                    { 24, "HN", "Hà Nội" },
                    { 25, "HUE", "Huế" },
                    { 26, "LCH", "Lai Châu" },
                    { 27, "DB", "Điện Biên" },
                    { 28, "SL", "Sơn La" },
                    { 29, "LS", "Lạng Sơn" },
                    { 30, "QNI", "Quảng Ninh" },
                    { 31, "TH", "Thanh Hóa" },
                    { 32, "NA", "Nghệ An" },
                    { 33, "HT", "Hà Tĩnh" },
                    { 34, "CB", "Cao Bằng" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLogs_DeviceId",
                table: "DeviceLogs",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_WarehouseId",
                table: "Devices",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_ProvinceId",
                table: "Warehouses",
                column: "ProvinceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceLogs");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "Provinces");
        }
    }
}
