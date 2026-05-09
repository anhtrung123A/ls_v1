using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class CreateClassSessionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "class_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    class_id = table.Column<long>(type: "bigint", nullable: false),
                    session_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    teacher_id = table.Column<long>(type: "bigint", nullable: true),
                    room_id = table.Column<long>(type: "bigint", nullable: true),
                    type = table.Column<sbyte>(type: "tinyint", nullable: false, defaultValue: (sbyte)1),
                    online_link = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<sbyte>(type: "tinyint", nullable: false, defaultValue: (sbyte)1),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_class_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_sessions_classes_class_id",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_class_sessions_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_class_sessions_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_class_sessions_users_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_class_sessions_class_id",
                table: "class_sessions",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_sessions_created_by",
                table: "class_sessions",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_class_sessions_room_id",
                table: "class_sessions",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_sessions_session_date",
                table: "class_sessions",
                column: "session_date");

            migrationBuilder.CreateIndex(
                name: "IX_class_sessions_status",
                table: "class_sessions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_class_sessions_teacher_id",
                table: "class_sessions",
                column: "teacher_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class_sessions");
        }
    }
}
