using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReworkBranchUsersAddIdDropRoleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE `branch_users_new` (
                    `id` bigint unsigned NOT NULL AUTO_INCREMENT,
                    `user_id` bigint unsigned NOT NULL,
                    `branch_id` bigint unsigned NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
                    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
                    `created_by_user_id` bigint unsigned NULL,
                    `updated_by_user_id` bigint unsigned NULL,
                    `deleted_at` datetime(6) NULL,
                    CONSTRAINT `PK_branch_users_new` PRIMARY KEY (`id`),
                    CONSTRAINT `FK_branch_users_new_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE CASCADE,
                    CONSTRAINT `FK_branch_users_new_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;

                INSERT INTO `branch_users_new`
                    (`user_id`, `branch_id`, `status`, `created_at`, `updated_at`, `created_by_user_id`, `updated_by_user_id`, `deleted_at`)
                SELECT
                    `user_id`, `branch_id`, `status`, `created_at`, `updated_at`, `created_by_user_id`, `updated_by_user_id`, `deleted_at`
                FROM `branch_users`;

                CREATE INDEX `ix_branch_users_branch_id` ON `branch_users_new` (`branch_id`);
                CREATE INDEX `ix_branch_users_status` ON `branch_users_new` (`status`);
                CREATE UNIQUE INDEX `ux_branch_users_user_id_branch_id` ON `branch_users_new` (`user_id`, `branch_id`);

                DROP TABLE `branch_users`;
                RENAME TABLE `branch_users_new` TO `branch_users`;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE `branch_users_old` (
                    `user_id` bigint unsigned NOT NULL,
                    `branch_id` bigint unsigned NOT NULL,
                    `role_id` bigint unsigned NOT NULL,
                    `status` int NOT NULL DEFAULT 1,
                    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
                    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
                    `created_by_user_id` bigint unsigned NULL,
                    `updated_by_user_id` bigint unsigned NULL,
                    `deleted_at` datetime(6) NULL,
                    CONSTRAINT `PK_branch_users_old` PRIMARY KEY (`user_id`, `branch_id`),
                    CONSTRAINT `FK_branch_users_old_branches_branch_id` FOREIGN KEY (`branch_id`) REFERENCES `branches` (`id`) ON DELETE CASCADE,
                    CONSTRAINT `FK_branch_users_old_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_branch_users_old_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;

                INSERT INTO `branch_users_old`
                    (`user_id`, `branch_id`, `role_id`, `status`, `created_at`, `updated_at`, `created_by_user_id`, `updated_by_user_id`, `deleted_at`)
                SELECT
                    `user_id`, `branch_id`, 1, `status`, `created_at`, `updated_at`, `created_by_user_id`, `updated_by_user_id`, `deleted_at`
                FROM `branch_users`;

                CREATE INDEX `ix_branch_users_branch_id` ON `branch_users_old` (`branch_id`);
                CREATE INDEX `ix_branch_users_role_id` ON `branch_users_old` (`role_id`);
                CREATE INDEX `ix_branch_users_status` ON `branch_users_old` (`status`);

                DROP TABLE `branch_users`;
                RENAME TABLE `branch_users_old` TO `branch_users`;
                """);
        }
    }
}
