using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace app.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)"),
                    created_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    updated_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    role_id = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6)"),
                    created_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    updated_by_user_id = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "created_by_user_id", "deleted_at", "name", "updated_at", "updated_by_user_id" },
                values: new object[,]
                {
                    { 1ul, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "admin", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2ul, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "branch_manager", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3ul, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "operator", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4ul, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "teacher", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5ul, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "student", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6ul, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "parent", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.CreateIndex(
                name: "ux_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
