using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSalonEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "salon_employee",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "salon_employee",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "salon_employee",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_of_birth",
                table: "salon_employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "salon_employee",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_salon_employee_account_id",
                table: "salon_employee",
                column: "account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_account_salon_employee",
                table: "salon_employee",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_salon_employee",
                table: "salon_employee");

            migrationBuilder.DropIndex(
                name: "IX_salon_employee_account_id",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "address",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "date_of_birth",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "email",
                table: "salon_employee");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "salon_employee",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}
