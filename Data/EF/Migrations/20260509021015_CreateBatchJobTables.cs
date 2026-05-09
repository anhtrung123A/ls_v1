using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class CreateBatchJobTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "batch_job_executions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    job_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<sbyte>(type: "tinyint", nullable: false),
                    triggered_by = table.Column<sbyte>(type: "tinyint", nullable: true),
                    started_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    finished_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    duration_ms = table.Column<long>(type: "bigint", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    error_trace = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_job_executions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "batch_job_locks",
                columns: table => new
                {
                    job_name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    locked_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    locked_by = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expires_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_job_locks", x => x.job_name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "batch_job_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    execution_id = table.Column<long>(type: "bigint", nullable: false),
                    target_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    target_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<sbyte>(type: "tinyint", nullable: false),
                    error_message = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_job_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_batch_job_items_batch_job_executions_execution_id",
                        column: x => x.execution_id,
                        principalTable: "batch_job_executions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_batch_job_executions_job_name",
                table: "batch_job_executions",
                column: "job_name");

            migrationBuilder.CreateIndex(
                name: "IX_batch_job_executions_started_at",
                table: "batch_job_executions",
                column: "started_at");

            migrationBuilder.CreateIndex(
                name: "IX_batch_job_executions_status",
                table: "batch_job_executions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_batch_job_items_execution_id",
                table: "batch_job_items",
                column: "execution_id");

            migrationBuilder.CreateIndex(
                name: "IX_batch_job_items_status",
                table: "batch_job_items",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_batch_job_items_target_type_target_id",
                table: "batch_job_items",
                columns: new[] { "target_type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "IX_batch_job_locks_expires_at",
                table: "batch_job_locks",
                column: "expires_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "batch_job_items");

            migrationBuilder.DropTable(
                name: "batch_job_locks");

            migrationBuilder.DropTable(
                name: "batch_job_executions");
        }
    }
}
