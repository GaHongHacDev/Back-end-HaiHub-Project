using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRatinginEmployeetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "salon_employee",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "salon_employee",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingSum",
                table: "salon_employee",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "salon_employee");

            migrationBuilder.DropColumn(
                name: "RatingSum",
                table: "salon_employee");
        }
    }
}
