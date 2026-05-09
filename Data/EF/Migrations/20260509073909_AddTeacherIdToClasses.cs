using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherIdToClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "teacher_id",
                table: "classes",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_classes_teacher_id",
                table: "classes",
                column: "teacher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_classes_users_teacher_id",
                table: "classes",
                column: "teacher_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_classes_users_teacher_id",
                table: "classes");

            migrationBuilder.DropIndex(
                name: "IX_classes_teacher_id",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "teacher_id",
                table: "classes");
        }
    }
}
