using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddClassStudentIdToClassAttendances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "class_student_id",
                table: "class_attendances",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_class_attendances_class_student_id",
                table: "class_attendances",
                column: "class_student_id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_attendances_class_students_class_student_id",
                table: "class_attendances",
                column: "class_student_id",
                principalTable: "class_students",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_attendances_class_students_class_student_id",
                table: "class_attendances");

            migrationBuilder.DropIndex(
                name: "IX_class_attendances_class_student_id",
                table: "class_attendances");

            migrationBuilder.DropColumn(
                name: "class_student_id",
                table: "class_attendances");
        }
    }
}
