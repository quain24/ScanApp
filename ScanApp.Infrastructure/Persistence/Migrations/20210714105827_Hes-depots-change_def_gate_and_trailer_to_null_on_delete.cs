using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class Hesdepotschange_def_gate_and_trailer_to_null_on_delete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Depots_Gates_DefaultGateId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.DropForeignKey(
                name: "FK_Depots_Trailers_DefaultTrailerId",
                schema: "hub",
                table: "Depots");

            migrationBuilder.AddForeignKey(
                name: "FK_Depots_Gates_DefaultGateId",
                schema: "hub",
                table: "Depots",
                column: "DefaultGateId",
                principalSchema: "hub",
                principalTable: "Gates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Depots_Trailers_DefaultTrailerId",
                schema: "hub",
                table: "Depots",
                column: "DefaultTrailerId",
                principalSchema: "hub",
                principalTable: "Trailers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
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
    }
}
