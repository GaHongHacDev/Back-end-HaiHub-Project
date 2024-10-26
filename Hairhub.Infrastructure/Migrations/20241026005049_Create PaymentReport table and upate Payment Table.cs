using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatePaymentReporttableandupatePaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salon_owner_payment",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "reason_cancle",
                table: "payment");

            migrationBuilder.AddColumn<Guid>(
                name: "payment_report_id",
                table: "static_file",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "payment_date",
                table: "payment",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "payment_code",
                table: "payment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "appointment_id",
                table: "payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "payment_report",
                columns: table => new
                {
                    payment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reason_cancle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    confirm_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    number_account = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    bank_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_report", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_payment_paymentReport",
                        column: x => x.payment_id,
                        principalTable: "payment",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_static_file_payment_report_id",
                table: "static_file",
                column: "payment_report_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_appointment_id",
                table: "payment",
                column: "appointment_id");

            migrationBuilder.AddForeignKey(
                name: "FK_account_payment",
                table: "payment",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_payment",
                table: "payment",
                column: "appointment_id",
                principalTable: "appointment",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_paymentReport_staticFile",
                table: "static_file",
                column: "payment_report_id",
                principalTable: "payment_report",
                principalColumn: "payment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_payment",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "FK_appointment_payment",
                table: "payment");

            migrationBuilder.DropForeignKey(
                name: "FK_paymentReport_staticFile",
                table: "static_file");

            migrationBuilder.DropTable(
                name: "payment_report");

            migrationBuilder.DropIndex(
                name: "IX_static_file_payment_report_id",
                table: "static_file");

            migrationBuilder.DropIndex(
                name: "IX_payment_appointment_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "payment_report_id",
                table: "static_file");

            migrationBuilder.DropColumn(
                name: "appointment_id",
                table: "payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "payment_date",
                table: "payment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "payment_code",
                table: "payment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason_cancle",
                table: "payment",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_salon_owner_payment",
                table: "payment",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id");
        }
    }
}
