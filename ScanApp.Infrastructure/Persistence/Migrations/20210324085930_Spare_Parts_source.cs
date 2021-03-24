using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class Spare_Parts_source : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceArticleId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SparePartTypes",
                schema: "sca",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartTypes", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_Name",
                schema: "sca",
                table: "SpareParts",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_SpareParts_SparePartTypes_Name",
                schema: "sca",
                table: "SpareParts",
                column: "Name",
                principalSchema: "sca",
                principalTable: "SparePartTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpareParts_SparePartTypes_Name",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.DropTable(
                name: "SparePartTypes",
                schema: "sca");

            migrationBuilder.DropIndex(
                name: "IX_SpareParts_Name",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.AlterColumn<string>(
                name: "StoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "SourceArticleId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
