using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    role_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    role_name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceHair",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    service_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHair", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 64, nullable: false),
                    username = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    password = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    role_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.id);
                    table.ForeignKey(
                        name: "FK_Account_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank_account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_account_admin",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    human_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank_account = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    bank_name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_account_customer",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "SalonOwner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    humand_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank_account = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    bank_name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonOwner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_account_salon_owner",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    commission_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    maintenance_fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    date_create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    admin_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                    table.ForeignKey(
                        name: "FK_admin_config",
                        column: x => x.admin_id,
                        principalTable: "Admin",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    total_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_appointment",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    payment_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    method_banking = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_customer_payment",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalonInformation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owner_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    service_hair_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    end_operational_hours = table.Column<DateTime>(type: "datetime2", nullable: true),
                    start_operational_hours = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_owner_salon_information",
                        column: x => x.owner_id,
                        principalTable: "SalonOwner",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_service_hair_salon_information",
                        column: x => x.service_hair_id,
                        principalTable: "ServiceHair",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalonEmployee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_information_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    human_id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonEmployee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salon_information_salon_employee",
                        column: x => x.salon_information_id,
                        principalTable: "SalonInformation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_information_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    is_system_create = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salon_information_voucher",
                        column: x => x.salon_information_id,
                        principalTable: "SalonInformation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppointmentDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    salon_employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    service_hair_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentDetail_Appointment_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "Appointment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_customer_appointment_detail",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_salon_employee_appointment_detail",
                        column: x => x.salon_employee_id,
                        principalTable: "SalonEmployee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_service_hair_appointment_detail",
                        column: x => x.service_hair_id,
                        principalTable: "ServiceHair",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_schedule",
                        column: x => x.employee_id,
                        principalTable: "SalonEmployee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_appointment_detail_feedback",
                        column: x => x.appointment_detail_id,
                        principalTable: "AppointmentDetail",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_customer_feedback",
                        column: x => x.customer_id,
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "FK__role__role_id__02084FDA",
                table: "Account",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Admin_AccountId",
                table: "Admin",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_customer_id",
                table: "Appointment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetail_appointment_id",
                table: "AppointmentDetail",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetail_customer_id",
                table: "AppointmentDetail",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetail_salon_employee_id",
                table: "AppointmentDetail",
                column: "salon_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentDetail_service_hair_id",
                table: "AppointmentDetail",
                column: "service_hair_id");

            migrationBuilder.CreateIndex(
                name: "IX_Config_admin_id",
                table: "Config",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_account_id",
                table: "Customer",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_appointment_detail_id",
                table: "Feedback",
                column: "appointment_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_customer_id",
                table: "Feedback",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_customer_id",
                table: "Payment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_SalonEmployee_salon_information_id",
                table: "SalonEmployee",
                column: "salon_information_id");

            migrationBuilder.CreateIndex(
                name: "IX_SalonInformation_owner_id",
                table: "SalonInformation",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_SalonInformation_service_hair_id",
                table: "SalonInformation",
                column: "service_hair_id");

            migrationBuilder.CreateIndex(
                name: "IX_SalonOwner_account_id",
                table: "SalonOwner",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_employee_id",
                table: "Schedule",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_salon_information_id",
                table: "Voucher",
                column: "salon_information_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "AppointmentDetail");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "SalonEmployee");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "SalonInformation");

            migrationBuilder.DropTable(
                name: "SalonOwner");

            migrationBuilder.DropTable(
                name: "ServiceHair");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
