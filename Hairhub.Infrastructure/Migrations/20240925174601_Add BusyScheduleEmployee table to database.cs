using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusyScheduleEmployeetabletodatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "busy_schedule_employee",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_busy_schedule_employee", x => x.id);
                    table.ForeignKey(
                        name: "FK_busy_schedule_employee_salon_employee",
                        column: x => x.employee_id,
                        principalTable: "salon_employee",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_busy_schedule_employee_employee_id",
                table: "busy_schedule_employee",
                column: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "busy_schedule_employee");
        }
    }
}
