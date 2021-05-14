using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class Spare_Parts_cleanup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoragePlaceId",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.AlterColumn<string>(
                name: "SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "StoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}