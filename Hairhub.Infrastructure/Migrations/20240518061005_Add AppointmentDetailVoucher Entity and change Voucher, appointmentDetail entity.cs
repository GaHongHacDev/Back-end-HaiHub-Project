using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentDetailVoucherEntityandchangeVoucherappointmentDetailentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_appointment_detail",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_salon_employee_appointment_detail",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_service_hair_appointment_detail",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_salon_information_voucher",
                table: "Voucher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher");

            migrationBuilder.RenameTable(
                name: "Voucher",
                newName: "vouchers");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "AppointmentDetail",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "AppointmentDetail",
                newName: "original_price");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentDetail_customer_id",
                table: "AppointmentDetail",
                newName: "IX_AppointmentDetail_CustomerId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "vouchers",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "vouchers",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "salon_information_id",
                table: "vouchers",
                newName: "SalonInformationId");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "vouchers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "is_system_create",
                table: "vouchers",
                newName: "IsSystemCreated");

            migrationBuilder.RenameColumn(
                name: "discount",
                table: "vouchers",
                newName: "MinimumOrderAmount");

            migrationBuilder.RenameIndex(
                name: "IX_Voucher_salon_information_id",
                table: "vouchers",
                newName: "IX_vouchers_SalonInformationId");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "AppointmentDetail",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "AppointmentDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId1",
                table: "AppointmentDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SalonEmployeeId1",
                table: "AppointmentDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceHairId1",
                table: "AppointmentDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discounted_price",
                table: "AppointmentDetail",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refesh_token",
                table: "Account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "vouchers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SalonInformationId",
                table: "vouchers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "vouchers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "vouchers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "vouchers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "vouchers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_vouchers",
                table: "vouchers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppointmentDetailVoucher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    voucher_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    applied_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    applied_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDetailVoucher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appointment_detail_appointment_detail_voucher",
                        column: x => x.appointment_detail_id,
                        principalTable: "AppointmentDetail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_voucher_appointment_detail_voucher",
                        column: x => x.voucher_id,
                        principalTable: "vouchers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetail_AppointmentId1",
                table: "AppointmentDetail",
                column: "AppointmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetail_SalonEmployeeId1",
                table: "AppointmentDetail",
                column: "SalonEmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetail_ServiceHairId1",
                table: "AppointmentDetail",
                column: "ServiceHairId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetailVoucher_appointment_detail_id",
                table: "AppointmentDetailVoucher",
                column: "appointment_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetailVoucher_voucher_id",
                table: "AppointmentDetailVoucher",
                column: "voucher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetail_Appointment_AppointmentId1",
                table: "AppointmentDetail",
                column: "AppointmentId1",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetail_Customer_CustomerId",
                table: "AppointmentDetail",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetail_SalonEmployee_SalonEmployeeId1",
                table: "AppointmentDetail",
                column: "SalonEmployeeId1",
                principalTable: "SalonEmployee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetail_SalonEmployee_salon_employee_id",
                table: "AppointmentDetail",
                column: "salon_employee_id",
                principalTable: "SalonEmployee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetail_ServiceHair_ServiceHairId1",
                table: "AppointmentDetail",
                column: "ServiceHairId1",
                principalTable: "ServiceHair",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentDetail_ServiceHair_service_hair_id",
                table: "AppointmentDetail",
                column: "service_hair_id",
                principalTable: "ServiceHair",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_vouchers_SalonInformation_SalonInformationId",
                table: "vouchers",
                column: "SalonInformationId",
                principalTable: "SalonInformation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetail_Appointment_AppointmentId1",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetail_Customer_CustomerId",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetail_SalonEmployee_SalonEmployeeId1",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetail_SalonEmployee_salon_employee_id",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetail_ServiceHair_ServiceHairId1",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentDetail_ServiceHair_service_hair_id",
                table: "AppointmentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_vouchers_SalonInformation_SalonInformationId",
                table: "vouchers");

            migrationBuilder.DropTable(
                name: "AppointmentDetailVoucher");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDetail_AppointmentId1",
                table: "AppointmentDetail");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDetail_SalonEmployeeId1",
                table: "AppointmentDetail");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentDetail_ServiceHairId1",
                table: "AppointmentDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vouchers",
                table: "vouchers");

            migrationBuilder.DropColumn(
                name: "AppointmentId1",
                table: "AppointmentDetail");

            migrationBuilder.DropColumn(
                name: "SalonEmployeeId1",
                table: "AppointmentDetail");

            migrationBuilder.DropColumn(
                name: "ServiceHairId1",
                table: "AppointmentDetail");

            migrationBuilder.DropColumn(
                name: "discounted_price",
                table: "AppointmentDetail");

            migrationBuilder.DropColumn(
                name: "refesh_token",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "vouchers");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "vouchers");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "vouchers");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "vouchers");

            migrationBuilder.RenameTable(
                name: "vouchers",
                newName: "Voucher");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "AppointmentDetail",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "original_price",
                table: "AppointmentDetail",
                newName: "price");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentDetail_CustomerId",
                table: "AppointmentDetail",
                newName: "IX_AppointmentDetail_customer_id");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Voucher",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Voucher",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "SalonInformationId",
                table: "Voucher",
                newName: "salon_information_id");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Voucher",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "MinimumOrderAmount",
                table: "Voucher",
                newName: "discount");

            migrationBuilder.RenameColumn(
                name: "IsSystemCreated",
                table: "Voucher",
                newName: "is_system_create");

            migrationBuilder.RenameIndex(
                name: "IX_vouchers_SalonInformationId",
                table: "Voucher",
                newName: "IX_Voucher_salon_information_id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "AppointmentDetail",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "customer_id",
                table: "AppointmentDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "Voucher",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "salon_information_id",
                table: "Voucher",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Voucher",
                table: "Voucher",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_appointment_detail",
                table: "AppointmentDetail",
                column: "customer_id",
                principalTable: "Customer",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_employee_appointment_detail",
                table: "AppointmentDetail",
                column: "salon_employee_id",
                principalTable: "SalonEmployee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_service_hair_appointment_detail",
                table: "AppointmentDetail",
                column: "service_hair_id",
                principalTable: "ServiceHair",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_information_voucher",
                table: "Voucher",
                column: "salon_information_id",
                principalTable: "SalonInformation",
                principalColumn: "Id");
        }
    }
}
