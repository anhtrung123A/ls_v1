using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AlignClassAttendancesWithDbml : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_attendances_enrollments_enrollment_id",
                table: "class_attendances");

            migrationBuilder.DropIndex(
                name: "IX_class_attendances_session_date",
                table: "class_attendances");

            migrationBuilder.DropColumn(
                name: "session_date",
                table: "class_attendances");

            migrationBuilder.DropColumn(
                name: "session_number",
                table: "class_attendances");

            migrationBuilder.DropColumn(
                name: "status",
                table: "class_attendances");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "class_attendances",
                newName: "absent_reason");

            migrationBuilder.RenameColumn(
                name: "enrollment_id",
                table: "class_attendances",
                newName: "class_session_id");

            migrationBuilder.RenameIndex(
                name: "IX_class_attendances_enrollment_id",
                table: "class_attendances",
                newName: "IX_class_attendances_class_session_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_absent",
                table: "class_attendances",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_class_attendances_class_sessions_class_session_id",
                table: "class_attendances",
                column: "class_session_id",
                principalTable: "class_sessions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_attendances_class_sessions_class_session_id",
                table: "class_attendances");

            migrationBuilder.DropColumn(
                name: "is_absent",
                table: "class_attendances");

            migrationBuilder.RenameColumn(
                name: "class_session_id",
                table: "class_attendances",
                newName: "enrollment_id");

            migrationBuilder.RenameColumn(
                name: "absent_reason",
                table: "class_attendances",
                newName: "note");

            migrationBuilder.RenameIndex(
                name: "IX_class_attendances_class_session_id",
                table: "class_attendances",
                newName: "IX_class_attendances_enrollment_id");

            migrationBuilder.AddColumn<DateOnly>(
                name: "session_date",
                table: "class_attendances",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "session_number",
                table: "class_attendances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "class_attendances",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_class_attendances_session_date",
                table: "class_attendances",
                column: "session_date");

            migrationBuilder.AddForeignKey(
                name: "FK_class_attendances_enrollments_enrollment_id",
                table: "class_attendances",
                column: "enrollment_id",
                principalTable: "enrollments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
