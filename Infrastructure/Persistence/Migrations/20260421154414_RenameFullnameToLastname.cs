using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameFullnameToLastname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "fullname",
                table: "users",
                newName: "lastname");

            migrationBuilder.Sql("""
                ALTER TABLE `users`
                MODIFY COLUMN `firstname` varchar(255) NULL AFTER `id`,
                MODIFY COLUMN `lastname` varchar(255) NOT NULL AFTER `firstname`;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "lastname",
                table: "users",
                newName: "fullname");

            migrationBuilder.Sql("""
                ALTER TABLE `users`
                MODIFY COLUMN `fullname` varchar(255) NOT NULL AFTER `id`,
                MODIFY COLUMN `firstname` varchar(255) NULL AFTER `fullname`;
                """);
        }
    }
}
