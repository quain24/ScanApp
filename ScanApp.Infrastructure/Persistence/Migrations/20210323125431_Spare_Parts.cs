using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class Spare_Parts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoragePlaces",
                schema: "sca",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LocationId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "SpareParts",
                schema: "sca",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    SourceArticleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoragePlaceId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpareParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpareParts_StoragePlaces_StoragePlaceId",
                        column: x => x.StoragePlaceId,
                        principalSchema: "sca",
                        principalTable: "StoragePlaces",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpareParts",
                schema: "sca");

            migrationBuilder.DropTable(
                name: "StoragePlaces",
                schema: "sca");
        }
    }
}
