using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addNotificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_detail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false),
                    read_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_detail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Account_notification_detail",
                        column: x => x.AccountId,
                        principalTable: "account",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Appointment_notification_detail",
                        column: x => x.AppointmentId,
                        principalTable: "appointment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_notification_notification_detail",
                        column: x => x.NotificationId,
                        principalTable: "notification",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_detail_AccountId",
                table: "notification_detail",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_detail_AppointmentId",
                table: "notification_detail",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_detail_NotificationId",
                table: "notification_detail",
                column: "NotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_detail");

            migrationBuilder.DropTable(
                name: "notification");
        }
    }
}
