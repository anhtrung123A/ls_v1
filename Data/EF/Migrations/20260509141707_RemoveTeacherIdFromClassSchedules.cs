using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTeacherIdFromClassSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_schedules_users_teacher_id",
                table: "class_schedules");

            migrationBuilder.DropIndex(
                name: "IX_class_schedules_teacher_id",
                table: "class_schedules");

            migrationBuilder.DropColumn(
                name: "teacher_id",
                table: "class_schedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "teacher_id",
                table: "class_schedules",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_class_schedules_teacher_id",
                table: "class_schedules",
                column: "teacher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_schedules_users_teacher_id",
                table: "class_schedules",
                column: "teacher_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
