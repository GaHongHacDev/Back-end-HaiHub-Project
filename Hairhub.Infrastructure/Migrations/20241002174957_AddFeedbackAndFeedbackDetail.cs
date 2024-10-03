using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackAndFeedbackDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rating",
                table: "feedback_detail",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "feedback_id",
                table: "feedback_detail",
                newName: "FeedbackId");

            migrationBuilder.RenameColumn(
                name: "appointment_detail_id",
                table: "feedback_detail",
                newName: "AppointmentDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_feedback_detail_feedback_id",
                table: "feedback_detail",
                newName: "IX_feedback_detail_FeedbackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "feedback_detail",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "FeedbackId",
                table: "feedback_detail",
                newName: "feedback_id");

            migrationBuilder.RenameColumn(
                name: "AppointmentDetailId",
                table: "feedback_detail",
                newName: "appointment_detail_id");

            migrationBuilder.RenameIndex(
                name: "IX_feedback_detail_FeedbackId",
                table: "feedback_detail",
                newName: "IX_feedback_detail_feedback_id");
        }
    }
}
