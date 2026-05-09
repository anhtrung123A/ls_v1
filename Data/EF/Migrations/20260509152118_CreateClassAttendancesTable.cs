using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class CreateClassAttendancesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "class_attendances",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    enrollment_id = table.Column<long>(type: "bigint", nullable: false),
                    session_date = table.Column<DateOnly>(type: "date", nullable: false),
                    session_number = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    note = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recorded_by = table.Column<long>(type: "bigint", nullable: true),
                    recorded_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class_attendances", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_attendances_enrollments_enrollment_id",
                        column: x => x.enrollment_id,
                        principalTable: "enrollments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_class_attendances_users_recorded_by",
                        column: x => x.recorded_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_class_attendances_enrollment_id",
                table: "class_attendances",
                column: "enrollment_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_attendances_recorded_by",
                table: "class_attendances",
                column: "recorded_by");

            migrationBuilder.CreateIndex(
                name: "IX_class_attendances_session_date",
                table: "class_attendances",
                column: "session_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class_attendances");
        }
    }
}
