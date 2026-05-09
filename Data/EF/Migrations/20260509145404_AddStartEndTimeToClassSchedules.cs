using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddStartEndTimeToClassSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "end_time",
                table: "class_schedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "start_time",
                table: "class_schedules",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_time",
                table: "class_schedules");

            migrationBuilder.DropColumn(
                name: "start_time",
                table: "class_schedules");
        }
    }
}
