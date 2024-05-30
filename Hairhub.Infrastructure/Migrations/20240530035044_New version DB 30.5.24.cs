using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewversionDB30524 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salon_information_service_hair",
                table: "service_hair");

            migrationBuilder.DropIndex(
                name: "IX_service_hair_salon_information_id",
                table: "service_hair");

            migrationBuilder.DropColumn(
                name: "salon_information_id",
                table: "service_hair");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "appointment_detail",
                newName: "start_time");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "appointment_detail",
                newName: "end_time");

            migrationBuilder.CreateTable(
                name: "service_employee",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    service_hair_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    salon_employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_employee", x => x.id);
                    table.ForeignKey(
                        name: "FK_salon_employee_service_employee",
                        column: x => x.salon_employee_id,
                        principalTable: "salon_employee",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_service_hair_service_employee",
                        column: x => x.service_hair_id,
                        principalTable: "service_hair",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_service_employee_salon_employee_id",
                table: "service_employee",
                column: "salon_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_employee_service_hair_id",
                table: "service_employee",
                column: "service_hair_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_employee");

            migrationBuilder.RenameColumn(
                name: "start_time",
                table: "appointment_detail",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "end_time",
                table: "appointment_detail",
                newName: "date");

            migrationBuilder.AddColumn<Guid>(
                name: "salon_information_id",
                table: "service_hair",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_hair_salon_information_id",
                table: "service_hair",
                column: "salon_information_id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_information_service_hair",
                table: "service_hair",
                column: "salon_information_id",
                principalTable: "salon_information",
                principalColumn: "id");
        }
    }
}
