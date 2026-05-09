using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class CreatePayrollTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "salary_configs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    salary_type = table.Column<sbyte>(type: "tinyint", nullable: false),
                    base_salary = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    teaching_rate = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    converted_lead_rate = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salary_configs", x => x.id);
                    table.ForeignKey(
                        name: "FK_salary_configs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "staff_kpi_records",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    staff_id = table.Column<long>(type: "bigint", nullable: false),
                    lead_id = table.Column<long>(type: "bigint", nullable: true),
                    month = table.Column<sbyte>(type: "tinyint", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<sbyte>(type: "tinyint", nullable: false, defaultValue: (sbyte)1),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    unit_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    total_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    note = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_kpi_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_staff_kpi_records_leads_lead_id",
                        column: x => x.lead_id,
                        principalTable: "leads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_staff_kpi_records_staff_staff_id",
                        column: x => x.staff_id,
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payrolls",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    salary_config_id = table.Column<long>(type: "bigint", nullable: true),
                    month = table.Column<sbyte>(type: "tinyint", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    base_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    teaching_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    kpi_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    bonus_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    deduction_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    gross_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    net_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false, defaultValue: 0m),
                    status = table.Column<sbyte>(type: "tinyint", nullable: false, defaultValue: (sbyte)1),
                    generated_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    confirmed_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    paid_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payrolls", x => x.id);
                    table.ForeignKey(
                        name: "FK_payrolls_salary_configs_salary_config_id",
                        column: x => x.salary_config_id,
                        principalTable: "salary_configs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_payrolls_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_payrolls_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "payroll_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    payroll_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<sbyte>(type: "tinyint", nullable: false),
                    quantity = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    unit_amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    reference_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reference_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payroll_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_payroll_items_payrolls_payroll_id",
                        column: x => x.payroll_id,
                        principalTable: "payrolls",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_items_payroll_id",
                table: "payroll_items",
                column: "payroll_id");

            migrationBuilder.CreateIndex(
                name: "IX_payroll_items_reference_type_reference_id",
                table: "payroll_items",
                columns: new[] { "reference_type", "reference_id" });

            migrationBuilder.CreateIndex(
                name: "IX_payroll_items_type",
                table: "payroll_items",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_payrolls_created_by",
                table: "payrolls",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_payrolls_salary_config_id",
                table: "payrolls",
                column: "salary_config_id");

            migrationBuilder.CreateIndex(
                name: "IX_payrolls_status",
                table: "payrolls",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_payrolls_user_id_month_year",
                table: "payrolls",
                columns: new[] { "user_id", "month", "year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salary_configs_effective_from",
                table: "salary_configs",
                column: "effective_from");

            migrationBuilder.CreateIndex(
                name: "IX_salary_configs_is_active",
                table: "salary_configs",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_salary_configs_user_id",
                table: "salary_configs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_kpi_records_lead_id",
                table: "staff_kpi_records",
                column: "lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_kpi_records_staff_id",
                table: "staff_kpi_records",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_kpi_records_staff_id_month_year",
                table: "staff_kpi_records",
                columns: new[] { "staff_id", "month", "year" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payroll_items");

            migrationBuilder.DropTable(
                name: "staff_kpi_records");

            migrationBuilder.DropTable(
                name: "payrolls");

            migrationBuilder.DropTable(
                name: "salary_configs");
        }
    }
}
