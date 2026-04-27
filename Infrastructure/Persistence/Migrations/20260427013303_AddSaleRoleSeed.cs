using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleRoleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "created_by_user_id", "deleted_at", "name", "updated_at", "updated_by_user_id" },
                values: new object[] { 7ul, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "sale", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 7ul);
        }
    }
}
