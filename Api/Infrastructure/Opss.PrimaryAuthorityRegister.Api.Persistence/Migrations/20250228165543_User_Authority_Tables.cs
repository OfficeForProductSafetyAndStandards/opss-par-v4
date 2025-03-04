using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class User_Authority_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authority",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(1024)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authority", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegulatoryFunction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(1024)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegulatoryFunction", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorityUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserIdentityId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorityUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorityUser_Authority_AuthorityId",
                        column: x => x.AuthorityId,
                        principalTable: "Authority",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuthorityUser_UserIdentities_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuthorityRegulatoryFunction",
                columns: table => new
                {
                    AuthoritiesId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegulatoryFunctionsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorityRegulatoryFunction", x => new { x.AuthoritiesId, x.RegulatoryFunctionsId });
                    table.ForeignKey(
                        name: "FK_AuthorityRegulatoryFunction_Authority_AuthoritiesId",
                        column: x => x.AuthoritiesId,
                        principalTable: "Authority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorityRegulatoryFunction_RegulatoryFunction_RegulatoryFu~",
                        column: x => x.RegulatoryFunctionsId,
                        principalTable: "RegulatoryFunction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorityRegulatoryFunction_RegulatoryFunctionsId",
                table: "AuthorityRegulatoryFunction",
                column: "RegulatoryFunctionsId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorityUser_AuthorityId",
                table: "AuthorityUser",
                column: "AuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorityUser_UserIdentityId",
                table: "AuthorityUser",
                column: "UserIdentityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorityRegulatoryFunction");

            migrationBuilder.DropTable(
                name: "AuthorityUser");

            migrationBuilder.DropTable(
                name: "RegulatoryFunction");

            migrationBuilder.DropTable(
                name: "Authority");
        }
    }
}
