using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatenewDBAppointment_detail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description_service_hair",
                table: "appointment_detail",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "img_service_hair",
                table: "appointment_detail",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "price_service_hair",
                table: "appointment_detail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "service_name",
                table: "appointment_detail",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "time_service_hair",
                table: "appointment_detail",
                type: "decimal(18,2)",
                maxLength: 25,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description_service_hair",
                table: "appointment_detail");

            migrationBuilder.DropColumn(
                name: "img_service_hair",
                table: "appointment_detail");

            migrationBuilder.DropColumn(
                name: "price_service_hair",
                table: "appointment_detail");

            migrationBuilder.DropColumn(
                name: "service_name",
                table: "appointment_detail");

            migrationBuilder.DropColumn(
                name: "time_service_hair",
                table: "appointment_detail");
        }
    }
}
