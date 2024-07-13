using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewDBupdatetableconfigpaymentandappointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "commission_rate",
                table: "payment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "pakage_fee",
                table: "payment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pakage_name",
                table: "payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "pakage_fee",
                table: "config",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "number_of_day",
                table: "config",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "config",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<decimal>(
                name: "commission_rate",
                table: "config",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commission_rate",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "pakage_fee",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "pakage_name",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "commission_rate",
                table: "config");

            migrationBuilder.AlterColumn<decimal>(
                name: "pakage_fee",
                table: "config",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "number_of_day",
                table: "config",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "config",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);
        }
    }
}
