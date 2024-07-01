using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseupdateforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_report_static_file",
                table: "static_file");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "salon_information",
                newName: "name");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "salon_information",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_static_file_report_id",
                table: "static_file",
                column: "report_id");

            migrationBuilder.AddForeignKey(
                name: "FK_report_static_file",
                table: "static_file",
                column: "report_id",
                principalTable: "report",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_report_static_file",
                table: "static_file");

            migrationBuilder.DropIndex(
                name: "IX_static_file_report_id",
                table: "static_file");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "salon_information",
                newName: "phone");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "salon_information",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_report_static_file",
                table: "static_file",
                column: "feed_back_id",
                principalTable: "report",
                principalColumn: "id");
        }
    }
}
