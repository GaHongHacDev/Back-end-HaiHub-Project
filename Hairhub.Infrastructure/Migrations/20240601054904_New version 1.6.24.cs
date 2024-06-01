using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Newversion1624 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_operational_hours",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "start_operational_hours",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "human_id",
                table: "customer");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "schedule",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "start_time",
                table: "schedule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "end_time",
                table: "schedule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "salon_id",
                table: "schedule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_schedule_salon_id",
                table: "schedule",
                column: "salon_id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_information_schedule",
                table: "schedule",
                column: "salon_id",
                principalTable: "salon_information",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salon_information_schedule",
                table: "schedule");

            migrationBuilder.DropIndex(
                name: "IX_schedule_salon_id",
                table: "schedule");

            migrationBuilder.DropColumn(
                name: "salon_id",
                table: "schedule");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "schedule",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "start_time",
                table: "schedule",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "end_time",
                table: "schedule",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "end_operational_hours",
                table: "salon_information",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_operational_hours",
                table: "salon_information",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "human_id",
                table: "customer",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
