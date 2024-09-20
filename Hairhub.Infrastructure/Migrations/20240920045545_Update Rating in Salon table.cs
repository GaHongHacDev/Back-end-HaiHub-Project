using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRatinginSalontable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "salon_employee",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "RatingSum",
                table: "salon_employee",
                newName: "rating_sum");

            migrationBuilder.RenameColumn(
                name: "RatingCount",
                table: "salon_employee",
                newName: "rating_count");

            migrationBuilder.AlterColumn<decimal>(
                name: "total_rating",
                table: "salon_information",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rating",
                table: "salon_employee",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "rating_sum",
                table: "salon_employee",
                newName: "RatingSum");

            migrationBuilder.RenameColumn(
                name: "rating_count",
                table: "salon_employee",
                newName: "RatingCount");

            migrationBuilder.AlterColumn<int>(
                name: "total_rating",
                table: "salon_information",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
