using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseCategoriesAndCategoryIdToCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "category",
                table: "courses");

            migrationBuilder.AddColumn<long>(
                name: "category_id",
                table: "courses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "course_categories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_categories", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "course_categories",
                columns: new[] { "id", "description", "is_active", "name", "slug", "sort_order" },
                values: new object[,]
                {
                    { 1L, null, true, "IELTS", "ielts", 1 },
                    { 2L, null, true, "TOEIC", "toeic", 2 },
                    { 3L, null, true, "TOEFL", "toefl", 3 },
                    { 4L, null, true, "English Communication", "english-communication", 4 },
                    { 5L, null, true, "Business English", "business-english", 5 },
                    { 6L, null, true, "Academic English", "academic-english", 6 },
                    { 7L, null, true, "Kids English", "kids-english", 7 },
                    { 8L, null, true, "Grammar", "grammar", 8 },
                    { 9L, null, true, "Pronunciation", "pronunciation", 9 },
                    { 10L, null, true, "Listening", "listening", 10 },
                    { 11L, null, true, "Speaking", "speaking", 11 },
                    { 12L, null, true, "Reading", "reading", 12 },
                    { 13L, null, true, "Writing", "writing", 13 },
                    { 14L, null, true, "SAT English", "sat-english", 14 },
                    { 15L, null, true, "Cambridge English", "cambridge-english", 15 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_courses_category_id",
                table: "courses",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_categories_slug",
                table: "course_categories",
                column: "slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_courses_course_categories_category_id",
                table: "courses",
                column: "category_id",
                principalTable: "course_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_course_categories_category_id",
                table: "courses");

            migrationBuilder.DropTable(
                name: "course_categories");

            migrationBuilder.DropIndex(
                name: "IX_courses_category_id",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "courses");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "courses",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
