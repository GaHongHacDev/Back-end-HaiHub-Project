using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewdatabaseUpdateappointmenttableinQrCodeImgfieldandfixnameofreasoncancelfileinappointmenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reason_report",
                table: "appointment",
                newName: "reason_cancel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "reason_cancel",
                table: "appointment",
                newName: "reason_report");
        }
    }
}
