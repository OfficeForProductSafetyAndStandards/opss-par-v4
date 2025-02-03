using Microsoft.EntityFrameworkCore.Migrations;
using Opss.PrimaryAuthorityRegister.Api.Persistence.Helpers;

#nullable disable

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TestData_CustomMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuplicatedData",
                table: "TestData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(MigrationHelper.ReadMigrationFile("20250203105141_TestData_CustomMigration.up.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuplicatedData",
                table: "TestData");
        }
    }
}
