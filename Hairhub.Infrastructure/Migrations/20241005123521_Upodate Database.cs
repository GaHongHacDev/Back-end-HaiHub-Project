using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpodateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rating",
                table: "feedback_detail",
                newName: "rating_detail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rating_detail",
                table: "feedback_detail",
                newName: "rating");
        }
    }
}
