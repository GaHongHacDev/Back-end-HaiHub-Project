using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabase25624updatenumberofreportedinSalonInformationandCustomertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reason_report",
                table: "report");

            migrationBuilder.AddColumn<int>(
                name: "number_of_reported",
                table: "salon_information",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description_admin",
                table: "report",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_of_report",
                table: "customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_report_appointment_id",
                table: "report",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_customer_id",
                table: "report",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_salon_id",
                table: "report",
                column: "salon_id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_report",
                table: "report",
                column: "appointment_id",
                principalTable: "appointment",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_report",
                table: "report",
                column: "customer_id",
                principalTable: "customer",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_report",
                table: "report",
                column: "salon_id",
                principalTable: "salon_information",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_report",
                table: "report");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_report",
                table: "report");

            migrationBuilder.DropForeignKey(
                name: "FK_salon_report",
                table: "report");

            migrationBuilder.DropIndex(
                name: "IX_report_appointment_id",
                table: "report");

            migrationBuilder.DropIndex(
                name: "IX_report_customer_id",
                table: "report");

            migrationBuilder.DropIndex(
                name: "IX_report_salon_id",
                table: "report");

            migrationBuilder.DropColumn(
                name: "number_of_reported",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "description_admin",
                table: "report");

            migrationBuilder.DropColumn(
                name: "number_of_report",
                table: "customer");

            migrationBuilder.AddColumn<string>(
                name: "reason_report",
                table: "report",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
