using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch_users",
                columns: table => new
                {
                    user_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    branch_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    role_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)"),
                    created_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    updated_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branch_users", x => new { x.user_id, x.branch_id });
                    table.ForeignKey(
                        name: "FK_branch_users_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_branch_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_branch_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_branch_users_branch_id",
                table: "branch_users",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_users_role_id",
                table: "branch_users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_branch_users_status",
                table: "branch_users",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "branch_users");
        }
    }
}
