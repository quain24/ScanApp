using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class HesDeparture_plansAdd_timezone_and_IsAllDay_fields_to_occurrence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndDateTimeZone",
                schema: "hub",
                table: "DeparturePlans",
                type: "nvarchar(256)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAllDay",
                schema: "hub",
                table: "DeparturePlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StartDateTimeZone",
                schema: "hub",
                table: "DeparturePlans",
                type: "nvarchar(256)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateTimeZone",
                schema: "hub",
                table: "DeparturePlans");

            migrationBuilder.DropColumn(
                name: "IsAllDay",
                schema: "hub",
                table: "DeparturePlans");

            migrationBuilder.DropColumn(
                name: "StartDateTimeZone",
                schema: "hub",
                table: "DeparturePlans");
        }
    }
}
