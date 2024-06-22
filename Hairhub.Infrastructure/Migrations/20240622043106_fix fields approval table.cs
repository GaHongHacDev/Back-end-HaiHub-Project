using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixfieldsapprovaltable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.RenameColumn(
                name: "create_date",
                table: "admin",
                newName: "img");

            migrationBuilder.RenameIndex(
                name: "IX_admin_staff_id",
                table: "admin",
                newName: "IX_admin_account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "img",
                table: "admin",
                newName: "create_date");

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
        }
    }
}
