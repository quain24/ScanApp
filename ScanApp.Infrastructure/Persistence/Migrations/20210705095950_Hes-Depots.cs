using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class HesDepots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hub");

            migrationBuilder.CreateTable(
                name: "Depots",
                schema: "hub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StreetName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "This Row version is converted to 'Version' object in ScanApp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Depots_Id",
                schema: "hub",
                table: "Depots",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Depots",
                schema: "hub");
        }
    }
}