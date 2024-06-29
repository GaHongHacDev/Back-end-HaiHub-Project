using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseCreateReporttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "report_id",
                table: "static_file",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_report_by_customer",
                table: "appointment",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_report_by_salon",
                table: "appointment",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_name_report = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    reason_report = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    time_confirm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_report_static_file",
                table: "static_file",
                column: "feed_back_id",
                principalTable: "report",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_report_static_file",
                table: "static_file");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropColumn(
                name: "report_id",
                table: "static_file");

            migrationBuilder.DropColumn(
                name: "is_report_by_customer",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "is_report_by_salon",
                table: "appointment");
        }
    }
}
