using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class HesDeparture_plansOccurrences_base_and_departure_entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeparturePlans",
                schema: "hub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    DepotId = table.Column<int>(type: "int", nullable: false),
                    TrailerTypeId = table.Column<int>(type: "int", nullable: false),
                    GateId = table.Column<int>(type: "int", nullable: false),
                    ArrivalTimeAtDepotDay = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Field is mapped to ScanApp 'Day' enumeration."),
                    ArrivalTimeAtDepotTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "This Row version is converted to 'Version' object in ScanApp"),
                    StartDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecurrenceType = table.Column<int>(type: "int", nullable: false),
                    RecurrenceInterval = table.Column<int>(type: "int", nullable: true),
                    RecurrenceCountLimit = table.Column<int>(type: "int", nullable: true),
                    RecurrenceEndDateUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RecurrenceByDay = table.Column<int>(type: "int", nullable: true),
                    RecurrenceByMonthDay = table.Column<int>(type: "int", nullable: true),
                    RecurrenceByMonth = table.Column<int>(type: "int", nullable: true),
                    RecurrenceOnWeek = table.Column<int>(type: "int", nullable: true),
                    ExceptionsToPatternOccurrenceUTC = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Timestamps stored in this column are in UTC time format."),
                    RecurrenceExceptionOfId = table.Column<int>(type: "int", nullable: true),
                    RecurrenceExceptionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsException = table.Column<bool>(type: "bit", nullable: false, computedColumnSql: "CASE WHEN ([RecurrenceExceptionOfId] IS NULL) OR ([RecurrenceExceptionDate] IS NULL) THEN 0 ELSE 1 END", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeparturePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeparturePlans_DeparturePlans_RecurrenceExceptionOfId",
                        column: x => x.RecurrenceExceptionOfId,
                        principalSchema: "hub",
                        principalTable: "DeparturePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeparturePlans_Depots_DepotId",
                        column: x => x.DepotId,
                        principalSchema: "hub",
                        principalTable: "Depots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeparturePlans_Gates_GateId",
                        column: x => x.GateId,
                        principalSchema: "hub",
                        principalTable: "Gates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeparturePlans_Trailers_TrailerTypeId",
                        column: x => x.TrailerTypeId,
                        principalSchema: "hub",
                        principalTable: "Trailers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                schema: "hub",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    StartDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true, comment: "This Row version is converted to 'Version' object in ScanApp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "DeparturePlanSeason",
                schema: "hub",
                columns: table => new
                {
                    DeparturePlansId = table.Column<int>(type: "int", nullable: false),
                    SeasonsName = table.Column<string>(type: "nvarchar(120)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeparturePlanSeason", x => new { x.DeparturePlansId, x.SeasonsName });
                    table.ForeignKey(
                        name: "FK_DeparturePlanSeason_DeparturePlans_DeparturePlansId",
                        column: x => x.DeparturePlansId,
                        principalSchema: "hub",
                        principalTable: "DeparturePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeparturePlanSeason_Seasons_SeasonsName",
                        column: x => x.SeasonsName,
                        principalSchema: "hub",
                        principalTable: "Seasons",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeparturePlans_DepotId",
                schema: "hub",
                table: "DeparturePlans",
                column: "DepotId");

            migrationBuilder.CreateIndex(
                name: "IX_DeparturePlans_GateId",
                schema: "hub",
                table: "DeparturePlans",
                column: "GateId");

            migrationBuilder.CreateIndex(
                name: "IX_DeparturePlans_RecurrenceExceptionOfId",
                schema: "hub",
                table: "DeparturePlans",
                column: "RecurrenceExceptionOfId");

            migrationBuilder.CreateIndex(
                name: "IX_DeparturePlans_TrailerTypeId",
                schema: "hub",
                table: "DeparturePlans",
                column: "TrailerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeparturePlanSeason_SeasonsName",
                schema: "hub",
                table: "DeparturePlanSeason",
                column: "SeasonsName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeparturePlanSeason",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "DeparturePlans",
                schema: "hub");

            migrationBuilder.DropTable(
                name: "Seasons",
                schema: "hub");
        }
    }
}
