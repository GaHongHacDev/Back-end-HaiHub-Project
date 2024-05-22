using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changetablesalonemployeesaloninformationconfigservicehair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_detail_appointment_detail_voucher",
                table: "appointment_detail_voucher");

            migrationBuilder.DropForeignKey(
                name: "FK_admin_config",
                table: "config");

            migrationBuilder.DropForeignKey(
                name: "FK_voucher_salon_information_SalonInformationId",
                table: "voucher");

            migrationBuilder.DropIndex(
                name: "IX_config_admin_id",
                table: "config");

            migrationBuilder.DropColumn(
                name: "admin_id",
                table: "config");

            migrationBuilder.DropColumn(
                name: "discounted_price",
                table: "appointment_detail");

            migrationBuilder.DropColumn(
                name: "original_price",
                table: "appointment_detail");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "voucher",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "voucher",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "SalonInformationId",
                table: "voucher",
                newName: "salon_information_id");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "voucher",
                newName: "modified_date");

            migrationBuilder.RenameColumn(
                name: "MinimumOrderAmount",
                table: "voucher",
                newName: "minimum_order_amount");

            migrationBuilder.RenameColumn(
                name: "IsSystemCreated",
                table: "voucher",
                newName: "is_system_created");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "voucher",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "voucher",
                newName: "expiry_date");

            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "voucher",
                newName: "discount_percentage");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "voucher",
                newName: "created_date");

            migrationBuilder.RenameIndex(
                name: "IX_voucher_SalonInformationId",
                table: "voucher",
                newName: "IX_voucher_salon_information_id");

            migrationBuilder.AddColumn<string>(
                name: "img",
                table: "service_hair",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "salon_information",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discounted_price",
                table: "appointment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "original_price",
                table: "appointment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_detail_appointment_detail_voucher",
                table: "appointment_detail_voucher",
                column: "appointment_detail_id",
                principalTable: "appointment",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_voucher_salon_information_salon_information_id",
                table: "voucher",
                column: "salon_information_id",
                principalTable: "salon_information",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointment_detail_appointment_detail_voucher",
                table: "appointment_detail_voucher");

            migrationBuilder.DropForeignKey(
                name: "FK_voucher_salon_information_salon_information_id",
                table: "voucher");

            migrationBuilder.DropColumn(
                name: "img",
                table: "service_hair");

            migrationBuilder.DropColumn(
                name: "name",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "discounted_price",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "original_price",
                table: "appointment");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "voucher",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "voucher",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "salon_information_id",
                table: "voucher",
                newName: "SalonInformationId");

            migrationBuilder.RenameColumn(
                name: "modified_date",
                table: "voucher",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "minimum_order_amount",
                table: "voucher",
                newName: "MinimumOrderAmount");

            migrationBuilder.RenameColumn(
                name: "is_system_created",
                table: "voucher",
                newName: "IsSystemCreated");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "voucher",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "expiry_date",
                table: "voucher",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "discount_percentage",
                table: "voucher",
                newName: "DiscountPercentage");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "voucher",
                newName: "CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_voucher_salon_information_id",
                table: "voucher",
                newName: "IX_voucher_SalonInformationId");

            migrationBuilder.AddColumn<Guid>(
                name: "admin_id",
                table: "config",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discounted_price",
                table: "appointment_detail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "original_price",
                table: "appointment_detail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_config_admin_id",
                table: "config",
                column: "admin_id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointment_detail_appointment_detail_voucher",
                table: "appointment_detail_voucher",
                column: "appointment_detail_id",
                principalTable: "appointment_detail",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_admin_config",
                table: "config",
                column: "admin_id",
                principalTable: "admin",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_voucher_salon_information_SalonInformationId",
                table: "voucher",
                column: "SalonInformationId",
                principalTable: "salon_information",
                principalColumn: "id");
        }
    }
}
