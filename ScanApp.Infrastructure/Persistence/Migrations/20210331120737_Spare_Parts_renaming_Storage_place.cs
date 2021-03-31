using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class Spare_Parts_renaming_Storage_place : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpareParts_StoragePlaces_StoragePlaceId",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.DropTable(
                name: "StoragePlaces",
                schema: "sca");

            migrationBuilder.DropIndex(
                name: "IX_SpareParts_StoragePlaceId",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.AlterColumn<string>(
                name: "StoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SparePartStoragePlaces",
                schema: "sca",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LocationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartStoragePlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SparePartStoragePlaces_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "sca",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                column: "SparePartStoragePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartStoragePlaces_LocationId",
                schema: "sca",
                table: "SparePartStoragePlaces",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SparePartStoragePlaces_Name_LocationId",
                schema: "sca",
                table: "SparePartStoragePlaces",
                columns: new[] { "Name", "LocationId" },
                unique: true,
                filter: "[Name] IS NOT NULL AND [LocationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SpareParts_SparePartStoragePlaces_SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                column: "SparePartStoragePlaceId",
                principalSchema: "sca",
                principalTable: "SparePartStoragePlaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpareParts_SparePartStoragePlaces_SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.DropTable(
                name: "SparePartStoragePlaces",
                schema: "sca");

            migrationBuilder.DropIndex(
                name: "IX_SpareParts_SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "SparePartStoragePlaceId",
                schema: "sca",
                table: "SpareParts");

            migrationBuilder.AlterColumn<string>(
                name: "StoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "StoragePlaces",
                schema: "sca",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LocationId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoragePlaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoragePlaces_Locations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "sca",
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_StoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                column: "StoragePlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_LocationId",
                schema: "sca",
                table: "StoragePlaces",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlaces_Name_LocationId",
                schema: "sca",
                table: "StoragePlaces",
                columns: new[] { "Name", "LocationId" },
                unique: true,
                filter: "[Name] IS NOT NULL AND [LocationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_SpareParts_StoragePlaces_StoragePlaceId",
                schema: "sca",
                table: "SpareParts",
                column: "StoragePlaceId",
                principalSchema: "sca",
                principalTable: "StoragePlaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
