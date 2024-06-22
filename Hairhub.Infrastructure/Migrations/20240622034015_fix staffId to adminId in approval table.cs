using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixstaffIdtoadminIdinapprovaltable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "staff_id",
                table: "approval",
                newName: "admin_id");

            migrationBuilder.RenameIndex(
                name: "IX_approval_staff_id",
                table: "approval",
                newName: "IX_approval_admin_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "admin_id",
                table: "approval",
                newName: "staff_id");

            migrationBuilder.RenameIndex(
                name: "IX_approval_admin_id",
                table: "approval",
                newName: "IX_approval_staff_id");
        }
    }
}
