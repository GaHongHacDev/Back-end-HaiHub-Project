using System;
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
            migrationBuilder.AddColumn<DateTime>(
                name: "day_of_birth",
                table: "SalonOwner",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "SalonOwner",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "day_of_birth",
                table: "SalonEmployee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "SalonEmployee",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "day_of_birth",
                table: "Customer",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "Customer",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "day_of_birth",
                table: "Admin",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "gender",
                table: "Admin",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "day_of_birth",
                table: "SalonOwner");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "SalonOwner");

            migrationBuilder.DropColumn(
                name: "day_of_birth",
                table: "SalonEmployee");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "SalonEmployee");

            migrationBuilder.DropColumn(
                name: "day_of_birth",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "day_of_birth",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "gender",
                table: "Admin");
        }
    }
}
