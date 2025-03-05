using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PartnershipApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartnershipApplication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorityId = table.Column<Guid>(type: "uuid", nullable: true),
                    PartnershipType = table.Column<string>(type: "varchar(128)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnershipApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnershipApplication_Authority_AuthorityId",
                        column: x => x.AuthorityId,
                        principalTable: "Authority",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnershipApplication_AuthorityId",
                table: "PartnershipApplication",
                column: "AuthorityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnershipApplication");
        }
    }
}
