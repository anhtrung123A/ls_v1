using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesTableAndUserAvatarFileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "avatar_file_id",
                table: "users",
                type: "bigint unsigned",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    object_key = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    bucket_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content_type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    size = table.Column<long>(type: "bigint", nullable: true),
                    checksum = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_public = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    metadata = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_users_avatar_file_id",
                table: "users",
                column: "avatar_file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_files_avatar_file_id",
                table: "users",
                column: "avatar_file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_files_avatar_file_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropIndex(
                name: "ix_users_avatar_file_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "avatar_file_id",
                table: "users");
        }
    }
}
