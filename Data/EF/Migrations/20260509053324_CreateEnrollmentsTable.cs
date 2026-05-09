using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class CreateEnrollmentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "enrollments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    student_id = table.Column<long>(type: "bigint", nullable: false),
                    class_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<sbyte>(type: "tinyint", nullable: false, defaultValue: (sbyte)1),
                    tuition_fee = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    discount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    discount_reason = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    final_fee = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    enrolled_by = table.Column<long>(type: "bigint", nullable: true),
                    enrolled_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    completed_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrollments", x => x.id);
                    table.ForeignKey(
                        name: "FK_enrollments_classes_class_id",
                        column: x => x.class_id,
                        principalTable: "classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_enrollments_staff_enrolled_by",
                        column: x => x.enrolled_by,
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_enrollments_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_class_id",
                table: "enrollments",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_enrolled_by",
                table: "enrollments",
                column: "enrolled_by");

            migrationBuilder.CreateIndex(
                name: "IX_enrollments_student_id_class_id",
                table: "enrollments",
                columns: new[] { "student_id", "class_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "enrollments");
        }
    }
}
