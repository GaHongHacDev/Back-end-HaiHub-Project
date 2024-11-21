using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Newversion1062024Updateconfigpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commission_rate",
                table: "config");

            migrationBuilder.DropColumn(
                name: "maintenance_fee",
                table: "config");

            migrationBuilder.AddColumn<Guid>(
                name: "config_id",
                table: "payment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "config",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_create",
                table: "config",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "config",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "pakage_fee",
                table: "config",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "pakage_name",
                table: "config",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_payment_config_id",
                table: "payment",
                column: "config_id");

            migrationBuilder.AddForeignKey(
                name: "FK_config_payment",
                table: "payment",
                column: "config_id",
                principalTable: "config",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_config_payment",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_config_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "config_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "description",
                table: "config");

            migrationBuilder.DropColumn(
                name: "pakage_fee",
                table: "config");

            migrationBuilder.DropColumn(
                name: "pakage_name",
                table: "config");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "config",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_create",
                table: "config",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "commission_rate",
                table: "config",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "maintenance_fee",
                table: "config",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
