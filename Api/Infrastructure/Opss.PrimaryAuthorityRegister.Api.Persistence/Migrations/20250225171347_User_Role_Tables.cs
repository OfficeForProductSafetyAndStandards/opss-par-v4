using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opss.PrimaryAuthorityRegister.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class User_Role_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserIdentities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailAddress = table.Column<string>(type: "varchar(320)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIdentities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleUserIdentity",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserIdentitiesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUserIdentity", x => new { x.RolesId, x.UserIdentitiesId });
                    table.ForeignKey(
                        name: "FK_RoleUserIdentity_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUserIdentity_UserIdentities_UserIdentitiesId",
                        column: x => x.UserIdentitiesId,
                        principalTable: "UserIdentities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleUserIdentity_UserIdentitiesId",
                table: "RoleUserIdentity",
                column: "UserIdentitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentities_EmailAddress",
                table: "UserIdentities",
                column: "EmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleUserIdentity");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserIdentities");
        }
    }
}
