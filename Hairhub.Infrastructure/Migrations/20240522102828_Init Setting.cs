using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "config",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    commission_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    maintenance_fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    date_create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "otp",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    otp_key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    expire_time = table.Column<double>(type: "float", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    type_otp = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_otp", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 64, nullable: false),
                    role_name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    username = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    password = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 64, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true),
                    token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    refesh_token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_acount",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "admin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    day_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank_account = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bank_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_admin",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    day_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_customer", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_customer",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "salon_owner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    day_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_salon_owner", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_salon_owner",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    total_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    original_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discounted_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_appointment",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    payment_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    method_banking = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_payment",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "salon_information",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owner_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    end_operational_hours = table.Column<DateTime>(type: "datetime2", nullable: true),
                    start_operational_hours = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_license = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_information", x => x.id);
                    table.ForeignKey(
                        name: "FK_owner_salon_information",
                        column: x => x.owner_id,
                        principalTable: "salon_owner",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "salon_employee",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_information_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    day_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_employee", x => x.id);
                    table.ForeignKey(
                        name: "FK_salon_information_salon_employee",
                        column: x => x.salon_information_id,
                        principalTable: "salon_information",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "service_hair",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_information_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    service_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_hair", x => x.id);
                    table.ForeignKey(
                        name: "FK_salon_information_service_hair",
                        column: x => x.salon_information_id,
                        principalTable: "salon_information",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "voucher",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_information_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    minimum_order_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    discount_percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modified_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_system_created = table.Column<bool>(type: "bit", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voucher", x => x.id);
                    table.ForeignKey(
                        name: "FK_voucher_salon_information_salon_information_id",
                        column: x => x.salon_information_id,
                        principalTable: "salon_information",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "schedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    end_time = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_schedule",
                        column: x => x.employee_id,
                        principalTable: "salon_employee",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment_detail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    service_hair_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_detail_appointment",
                        column: x => x.appointment_id,
                        principalTable: "appointment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_appointment_detail_salon_employee",
                        column: x => x.salon_employee_id,
                        principalTable: "salon_employee",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_appointment_detail_service_hair",
                        column: x => x.service_hair_id,
                        principalTable: "service_hair",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment_detail_voucher",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    voucher_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    applied_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    applied_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_detail_voucher", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_detail_appointment_detail_voucher",
                        column: x => x.appointment_detail_id,
                        principalTable: "appointment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_voucher_appointment_detail_voucher",
                        column: x => x.voucher_id,
                        principalTable: "voucher",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_detail_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedback", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_detail_feedback",
                        column: x => x.appointment_detail_id,
                        principalTable: "appointment_detail",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_customer_feedback",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_role_id",
                table: "account",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_admin_account_id",
                table: "admin",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_customer_id",
                table: "appointment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_detail_appointment_id",
                table: "appointment_detail",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_detail_salon_employee_id",
                table: "appointment_detail",
                column: "salon_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_detail_service_hair_id",
                table: "appointment_detail",
                column: "service_hair_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_detail_voucher_appointment_detail_id",
                table: "appointment_detail_voucher",
                column: "appointment_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_detail_voucher_voucher_id",
                table: "appointment_detail_voucher",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_account_id",
                table: "customer",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_appointment_detail_id",
                table: "feedback",
                column: "appointment_detail_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_customer_id",
                table: "feedback",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_customer_id",
                table: "payment",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_salon_employee_salon_information_id",
                table: "salon_employee",
                column: "salon_information_id");

            migrationBuilder.CreateIndex(
                name: "IX_salon_information_owner_id",
                table: "salon_information",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_salon_owner_account_id",
                table: "salon_owner",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_employee_id",
                table: "schedule",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_hair_salon_information_id",
                table: "service_hair",
                column: "salon_information_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_salon_information_id",
                table: "voucher",
                column: "salon_information_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "appointment_detail_voucher");

            migrationBuilder.DropTable(
                name: "config");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "otp");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "schedule");

            migrationBuilder.DropTable(
                name: "voucher");

            migrationBuilder.DropTable(
                name: "appointment_detail");

            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "salon_employee");

            migrationBuilder.DropTable(
                name: "service_hair");

            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "salon_information");

            migrationBuilder.DropTable(
                name: "salon_owner");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
