using Microsoft.EntityFrameworkCore.Migrations;

namespace ScanApp.Infrastructure.Persistence.Migrations
{
    public partial class Asp_security_claims_source_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                schema: "sca",
                table: "UserRoleClaims",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                schema: "sca",
                table: "UserRoleClaims",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                schema: "sca",
                table: "UserClaims",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                schema: "sca",
                table: "UserClaims",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ClaimsSource",
                schema: "sca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimsSource", x => x.Id);
                    table.UniqueConstraint("AK_ClaimsSource_Type_Value", x => new { x.Type, x.Value });
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleClaims_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserRoleClaims",
                columns: new[] { "ClaimType", "ClaimValue" });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserClaims",
                columns: new[] { "ClaimType", "ClaimValue" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_ClaimsSource_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserClaims",
                columns: new[] { "ClaimType", "ClaimValue" },
                principalSchema: "sca",
                principalTable: "ClaimsSource",
                principalColumns: new[] { "Type", "Value" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleClaims_ClaimsSource_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserRoleClaims",
                columns: new[] { "ClaimType", "ClaimValue" },
                principalSchema: "sca",
                principalTable: "ClaimsSource",
                principalColumns: new[] { "Type", "Value" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_ClaimsSource_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleClaims_ClaimsSource_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserRoleClaims");

            migrationBuilder.DropTable(
                name: "ClaimsSource",
                schema: "sca");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleClaims_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserRoleClaims");

            migrationBuilder.DropIndex(
                name: "IX_UserClaims_ClaimType_ClaimValue",
                schema: "sca",
                table: "UserClaims");

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                schema: "sca",
                table: "UserRoleClaims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                schema: "sca",
                table: "UserRoleClaims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimValue",
                schema: "sca",
                table: "UserClaims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClaimType",
                schema: "sca",
                table: "UserClaims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}