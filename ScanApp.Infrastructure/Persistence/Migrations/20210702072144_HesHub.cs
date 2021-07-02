using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class HesHub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hub");

            migrationBuilder.CreateTable(
                name: "HesDepots",
                schema: "hub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StreetNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhonePrefix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "This Row version is converted to 'Version' object in ScanApp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HesDepots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HesDepots_Id",
                schema: "hub",
                table: "HesDepots",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HesDepots",
                schema: "hub");
        }
    }
}
