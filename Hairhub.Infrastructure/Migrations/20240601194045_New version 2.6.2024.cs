using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Newversion262024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "day_of_birth",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "email",
                table: "salon_employee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "salon_employee",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "day_of_birth",
                table: "salon_employee",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "salon_employee",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }
    }
}
