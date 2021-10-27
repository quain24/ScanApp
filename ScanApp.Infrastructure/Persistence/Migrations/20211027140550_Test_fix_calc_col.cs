using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class Test_fix_calc_col : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsException",
                schema: "hub",
                table: "DeparturePlans",
                type: "bit",
                nullable: false,
                computedColumnSql: "CAST(CASE WHEN ([RecurrenceExceptionOfId] IS NULL) OR ([RecurrenceExceptionDate] IS NULL) THEN 0 ELSE 1 END AS BIT)",
                stored: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComputedColumnSql: "CASE WHEN ([RecurrenceExceptionOfId] IS NULL) OR ([RecurrenceExceptionDate] IS NULL) THEN 0 ELSE 1 END",
                oldStored: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsException",
                schema: "hub",
                table: "DeparturePlans",
                type: "bit",
                nullable: false,
                computedColumnSql: "CASE WHEN ([RecurrenceExceptionOfId] IS NULL) OR ([RecurrenceExceptionDate] IS NULL) THEN 0 ELSE 1 END",
                stored: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComputedColumnSql: "CAST(CASE WHEN ([RecurrenceExceptionOfId] IS NULL) OR ([RecurrenceExceptionDate] IS NULL) THEN 0 ELSE 1 END AS BIT)",
                oldStored: true);
        }
    }
}
