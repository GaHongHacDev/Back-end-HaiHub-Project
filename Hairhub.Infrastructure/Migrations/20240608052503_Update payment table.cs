using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updatepaymenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "payment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "payment_code",
                table: "payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "salon_id",
                table: "payment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_payment_salon_id",
                table: "payment",
                column: "salon_id");

            migrationBuilder.AddForeignKey(
                name: "FK_salon_payment",
                table: "payment",
                column: "salon_id",
                principalTable: "salon_information",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_salon_payment",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_salon_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "description",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "payment_code",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "salon_id",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "status",
                table: "payment");
        }
    }
}
