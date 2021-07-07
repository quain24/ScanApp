using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class HesDepots_gate_and_trailer_entites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Depots_Id",
                schema: "hub",
                table: "Depots");

            migrationBuilder.AddColumn<int>(
                name: "DefaultGateId",
                schema: "hub",
                table: "Depots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultTrailerId",
                schema: "hub",
                table: "Depots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DistanceFromHub",
                schema: "hub",
                table: "Depots",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Gates",
                schema: "hub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "This Row version is converted to 'Version' object in ScanApp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trailers",
                schema: "hub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxVolume = table.Column<float>(type: "real", nullable: false),
                    MaxWeight = table.Column<float>(type: "real", nullable: false),
                    LoadingTime = table.Column<string>(type: "nvarchar(48)", nullable: false),
                    UnloadingTime = table.Column<string>(type: "nvarchar(48)", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "This Row version is converted to 'Version' object in ScanApp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Depots_DefaultGateId",
                schema: "hub",
                table: "Depots",
                column: "DefaultGateId");

            migrationBuilder.CreateIndex(
                name: "IX_Depots_DefaultTrailerId",
                schema: "hub",
                table: "Depots",
                column: "DefaultTrailerId");

            migrationBuilder.CreateIndex(
                name: "IX_Gates_Number",
                schema: "hub",
                table: "Gates",
                column: "Number",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Depots_Gates_DefaultGateId",
                schema: "hub",
                table: "Depots",
                column: "DefaultGateId",
                principalSchema: "hub",
                principalTable: "Gates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Depots_Trailers_DefaultTrailerId",
                schema: "hub",
                table: "Depots",
                column: "DefaultTrailerId",
                principalSchema: "hub",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depots_Gates_DefaultGateId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.DropForeignKey(
                name: "FK_Depots_Trailers_DefaultTrailerId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.DropTable(
                name: "Gates",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "Trailers",
                schema: "hub");

            migrationBuilder.DropIndex(
                name: "IX_Depots_DefaultGateId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.DropIndex(
                name: "IX_Depots_DefaultTrailerId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "DefaultGateId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "DefaultTrailerId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.DropColumn(
                name: "DistanceFromHub",
                schema: "hub",
                table: "Depots");

            migrationBuilder.CreateIndex(
                name: "IX_Depots_Id",
                schema: "hub",
                table: "Depots",
                column: "Id",
                unique: true);
        }
    }
}
