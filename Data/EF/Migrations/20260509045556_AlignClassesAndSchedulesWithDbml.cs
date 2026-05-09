using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AlignClassesAndSchedulesWithDbml : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_classes_rooms_room_id",
                table: "classes");

            migrationBuilder.DropForeignKey(
                name: "FK_classes_users_teacher_id",
                table: "classes");

            migrationBuilder.DropIndex(
                name: "IX_classes_room_id",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "online_link",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "classes");

            migrationBuilder.DropColumn(
                name: "schedule",
                table: "classes");

            migrationBuilder.RenameColumn(
                name: "teacher_id",
                table: "classes",
                newName: "created_by");

            migrationBuilder.RenameIndex(
                name: "IX_classes_teacher_id",
                table: "classes",
                newName: "IX_classes_created_by");

            migrationBuilder.RenameColumn(
                name: "effective_to",
                table: "class_schedules",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "effective_from",
                table: "class_schedules",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "day_of_week",
                table: "class_schedules",
                newName: "weekday");

            migrationBuilder.RenameIndex(
                name: "IX_class_schedules_class_id_day_of_week_start_time_end_time",
                table: "class_schedules",
                newName: "IX_class_schedules_class_id_weekday_start_time_end_time");

            migrationBuilder.Sql(
                "UPDATE classes SET class_code = CONCAT('CLS', LPAD(id, 6, '0')) " +
                "WHERE class_code IS NULL OR class_code = '';");

            migrationBuilder.AlterColumn<string>(
                name: "class_code",
                table: "classes",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "online_link",
                table: "class_schedules",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "room_id",
                table: "class_schedules",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "teacher_id",
                table: "class_schedules",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<sbyte>(
                name: "type",
                table: "class_schedules",
                type: "tinyint",
                nullable: false,
                defaultValue: (sbyte)1);

            migrationBuilder.CreateIndex(
                name: "IX_class_schedules_room_id",
                table: "class_schedules",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_schedules_teacher_id",
                table: "class_schedules",
                column: "teacher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_schedules_rooms_room_id",
                table: "class_schedules",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_class_schedules_users_teacher_id",
                table: "class_schedules",
                column: "teacher_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_classes_users_created_by",
                table: "classes",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_schedules_rooms_room_id",
                table: "class_schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_class_schedules_users_teacher_id",
                table: "class_schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_classes_users_created_by",
                table: "classes");

            migrationBuilder.DropIndex(
                name: "IX_class_schedules_room_id",
                table: "class_schedules");

            migrationBuilder.DropIndex(
                name: "IX_class_schedules_teacher_id",
                table: "class_schedules");

            migrationBuilder.DropColumn(
                name: "online_link",
                table: "class_schedules");

            migrationBuilder.DropColumn(
                name: "room_id",
                table: "class_schedules");

            migrationBuilder.DropColumn(
                name: "teacher_id",
                table: "class_schedules");

            migrationBuilder.DropColumn(
                name: "type",
                table: "class_schedules");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "classes",
                newName: "teacher_id");

            migrationBuilder.RenameIndex(
                name: "IX_classes_created_by",
                table: "classes",
                newName: "IX_classes_teacher_id");

            migrationBuilder.RenameColumn(
                name: "weekday",
                table: "class_schedules",
                newName: "day_of_week");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "class_schedules",
                newName: "effective_to");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "class_schedules",
                newName: "effective_from");

            migrationBuilder.RenameIndex(
                name: "IX_class_schedules_class_id_weekday_start_time_end_time",
                table: "class_schedules",
                newName: "IX_class_schedules_class_id_day_of_week_start_time_end_time");

            migrationBuilder.AlterColumn<string>(
                name: "class_code",
                table: "classes",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldMaxLength: 30)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "online_link",
                table: "classes",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "room_id",
                table: "classes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "schedule",
                table: "classes",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_classes_room_id",
                table: "classes",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_classes_rooms_room_id",
                table: "classes",
                column: "room_id",
                principalTable: "rooms",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_classes_users_teacher_id",
                table: "classes",
                column: "teacher_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
