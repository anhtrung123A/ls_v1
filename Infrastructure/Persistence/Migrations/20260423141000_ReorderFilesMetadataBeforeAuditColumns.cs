using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260423141000_ReorderFilesMetadataBeforeAuditColumns")]
    /// <inheritdoc />
    public partial class ReorderFilesMetadataBeforeAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE `files`
                  MODIFY COLUMN `metadata` json NULL AFTER `is_public`,
                  MODIFY COLUMN `created_at` datetime(6) NOT NULL AFTER `metadata`,
                  MODIFY COLUMN `created_by` bigint unsigned NULL AFTER `created_at`,
                  MODIFY COLUMN `deleted_at` datetime(6) NULL AFTER `created_by`;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE `files`
                  MODIFY COLUMN `created_at` datetime(6) NOT NULL AFTER `is_public`,
                  MODIFY COLUMN `created_by` bigint unsigned NULL AFTER `created_at`,
                  MODIFY COLUMN `deleted_at` datetime(6) NULL AFTER `created_by`,
                  MODIFY COLUMN `metadata` json NULL AFTER `deleted_at`;
                """);
        }
    }
}
