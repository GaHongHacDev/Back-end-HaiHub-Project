using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixingStaticFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img",
                table: "salon_information");

            migrationBuilder.AddColumn<Guid>(
                name: "salon_information_id",
                table: "static_file",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_static_file_salon_information_id",
                table: "static_file",
                column: "salon_information_id");

            migrationBuilder.AddForeignKey(
                name: "FK_saloninformation_static_file",
                table: "static_file",
                column: "salon_information_id",
                principalTable: "salon_information",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_saloninformation_static_file",
                table: "static_file");

            migrationBuilder.DropIndex(
                name: "IX_static_file_salon_information_id",
                table: "static_file");

            migrationBuilder.DropColumn(
                name: "salon_information_id",
                table: "static_file");

            migrationBuilder.AddColumn<string>(
                name: "img",
                table: "salon_information",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
