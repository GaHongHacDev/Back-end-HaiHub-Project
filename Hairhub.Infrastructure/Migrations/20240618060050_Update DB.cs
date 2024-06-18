using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "appointment_detail_voucher");

            migrationBuilder.DropColumn(
                name: "applied_amount",
                table: "appointment_detail_voucher");

            migrationBuilder.DropColumn(
                name: "applied_date",
                table: "appointment_detail_voucher");

            migrationBuilder.RenameColumn(
                name: "appointment_detail_id",
                table: "appointment_detail_voucher",
                newName: "appointment_id");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_detail_voucher_appointment_detail_id",
                table: "appointment_detail_voucher",
                newName: "IX_appointment_detail_voucher_appointment_id");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "appointment",
                newName: "start_date");

            migrationBuilder.AlterColumn<decimal>(
                name: "minimum_order_amount",
                table: "voucher",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "expiry_date",
                table: "voucher",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_percentage",
                table: "voucher",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_date",
                table: "voucher",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_date",
                table: "appointment",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_date",
                table: "appointment");

            migrationBuilder.RenameColumn(
                name: "appointment_id",
                table: "appointment_detail_voucher",
                newName: "appointment_detail_id");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_detail_voucher_appointment_id",
                table: "appointment_detail_voucher",
                newName: "IX_appointment_detail_voucher_appointment_detail_id");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "appointment",
                newName: "date");

            migrationBuilder.AlterColumn<decimal>(
                name: "minimum_order_amount",
                table: "voucher",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "expiry_date",
                table: "voucher",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_percentage",
                table: "voucher",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_date",
                table: "voucher",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "appointment_detail_voucher",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "applied_amount",
                table: "appointment_detail_voucher",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "applied_date",
                table: "appointment_detail_voucher",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
