using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReInstallDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "salon_information");

            migrationBuilder.DropColumn(
                name: "human_id",
                table: "salon_employee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "salon_information",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "salon_information",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "human_id",
                table: "salon_employee",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }
    }
}
