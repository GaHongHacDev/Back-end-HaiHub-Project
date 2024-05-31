using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatenewversionDB3152024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refesh_token",
                table: "account");

            migrationBuilder.DropColumn(
                name: "token",
                table: "account");

            migrationBuilder.CreateTable(
                name: "refresh_token_account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    access_token = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    expires = table.Column<DateTime>(type: "datetime2", maxLength: 64, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_refresh_token_account",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_account_account_id",
                table: "refresh_token_account",
                column: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_token_account");

            migrationBuilder.AddColumn<string>(
                name: "refesh_token",
                table: "account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "account",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
