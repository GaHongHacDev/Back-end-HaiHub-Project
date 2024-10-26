using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBinAppointmentAccountandPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salon_owner_payment",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_salon_owner_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "method_banking",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "salon_owner_id",
                table: "payment");

            migrationBuilder.AddColumn<Guid>(
                name: "account_id",
                table: "payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_type",
                table: "payment",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reason_cancle",
                table: "payment",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "appointment",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "balance",
                table: "account",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_payment_account_id",
                table: "payment",
                column: "account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_owner_payment",
                table: "payment",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salon_owner_payment",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_account_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "payment_type",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "reason_cancle",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "balance",
                table: "account");

            migrationBuilder.AddColumn<string>(
                name: "method_banking",
                table: "payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "salon_owner_id",
                table: "payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_payment_salon_owner_id",
                table: "payment",
                column: "salon_owner_id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_owner_payment",
                table: "payment",
                column: "salon_owner_id",
                principalTable: "salon_owner",
                principalColumn: "id");
        }
    }
}
