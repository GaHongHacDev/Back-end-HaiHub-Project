using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewversionDBAddadminapprovaltable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_admin",
                table: "admin");

            migrationBuilder.DropForeignKey(
                name: "FK_voucher_salon_information_salon_information_id",
                table: "voucher");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "address",
                table: "admin");

            migrationBuilder.DropColumn(
                name: "bank_account",
                table: "admin");

            migrationBuilder.DropColumn(
                name: "bank_name",
                table: "admin");

            migrationBuilder.DropColumn(
                name: "day_of_birth",
                table: "admin");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "admin");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "admin");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "admin",
                newName: "salon_id");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "admin",
                newName: "reason_reject");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "admin",
                newName: "staff_id");

            migrationBuilder.RenameIndex(
                name: "IX_admin_account_id",
                table: "admin",
                newName: "IX_admin_staff_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "salon_information_id",
                table: "voucher",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "salon_information",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "salon_id",
                table: "admin",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "reason_reject",
                table: "admin",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "create_date",
                table: "admin",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "approval",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    staff_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reason_reject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval", x => x.id);
                    table.ForeignKey(
                        name: "FK_approval_admin",
                        column: x => x.staff_id,
                        principalTable: "admin",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_approval_salon_information",
                        column: x => x.salon_id,
                        principalTable: "salon_information",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_approval_salon_id",
                table: "approval",
                column: "salon_id");

            migrationBuilder.CreateIndex(
                name: "IX_approval_staff_id",
                table: "approval",
                column: "staff_id");

            migrationBuilder.AddForeignKey(
                name: "FK_admin_account",
                table: "admin",
                column: "staff_id",
                principalTable: "account",
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
                name: "FK_admin_account",
                table: "admin");

            migrationBuilder.DropForeignKey(
                name: "FK_voucher_salon_information_salon_information_id",
                table: "voucher");

            migrationBuilder.DropTable(
                name: "approval");

            migrationBuilder.DropColumn(
                name: "status",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "create_date",
                table: "admin");

            migrationBuilder.RenameColumn(
                name: "staff_id",
                table: "admin",
                newName: "account_id");

            migrationBuilder.RenameColumn(
                name: "salon_id",
                table: "admin",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "reason_reject",
                table: "admin",
                newName: "email");

            migrationBuilder.RenameIndex(
                name: "IX_admin_staff_id",
                table: "admin",
                newName: "IX_admin_account_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "salon_information_id",
                table: "voucher",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "salon_information",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "admin",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "admin",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "admin",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bank_account",
                table: "admin",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "bank_name",
                table: "admin",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "day_of_birth",
                table: "admin",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "admin",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "admin",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_account_admin",
                table: "admin",
                column: "account_id",
                principalTable: "account",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_voucher_salon_information_salon_information_id",
                table: "voucher",
                column: "salon_information_id",
                principalTable: "salon_information",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
