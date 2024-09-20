using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddapprovedAtfieldtosalontable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "config",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    pakage_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    pakage_fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    date_create = table.Column<DateTime>(type: "datetime2", nullable: false),
                    commission_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    number_of_day = table.Column<int>(type: "int", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
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
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    role_id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 64, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
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
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admin", x => x.id);
                    table.ForeignKey(
                        name: "FK_admin_account",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    day_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    number_of_report = table.Column<int>(type: "int", nullable: true)
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
                name: "refresh_token_account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    access_token = table.Column<string>(type: "nvarchar(515)", maxLength: 515, nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    expires = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "salon_owner",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    account_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    day_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    email = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
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
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    original_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discounted_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    is_report_by_customer = table.Column<bool>(type: "bit", nullable: true),
                    is_report_by_salon = table.Column<bool>(type: "bit", nullable: true),
                    reason_cancel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    cancel_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    qr_code_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    commission_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
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
                name: "style_hair_customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_style_hair_customer", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_style_hair_customer",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    config_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    salon_owner_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payment_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    method_banking = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    payment_code = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    pakage_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    pakage_fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    commission_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
                    table.ForeignKey(
                        name: "FK_config_payment",
                        column: x => x.config_id,
                        principalTable: "config",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_salon_owner_payment",
                        column: x => x.salon_owner_id,
                        principalTable: "salon_owner",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "salon_information",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    owner_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    rate = table.Column<decimal>(type: "decimal(18,1)", nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(18,10)", maxLength: 150, nullable: false),
                    latitude = table.Column<decimal>(type: "decimal(18,10)", maxLength: 150, nullable: false),
                    total_rating = table.Column<int>(type: "int", nullable: false),
                    total_reviewer = table.Column<int>(type: "int", nullable: false),
                    number_of_reported = table.Column<int>(type: "int", nullable: true),
                    update_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
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
                name: "feedback",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedback", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_feedback",
                        column: x => x.appointment_id,
                        principalTable: "appointment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_customer_feedback",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "image_style",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    style_hair_customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    url_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_style", x => x.id);
                    table.ForeignKey(
                        name: "FK_image_style_style_hair_customer",
                        column: x => x.style_hair_customer_id,
                        principalTable: "style_hair_customer",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "approval",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    admin_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    reason_reject = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval", x => x.id);
                    table.ForeignKey(
                        name: "FK_approval_admin",
                        column: x => x.admin_id,
                        principalTable: "admin",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_approval_salon_information",
                        column: x => x.salon_id,
                        principalTable: "salon_information",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_name_report = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    reason_report = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    create_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    time_confirm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    description_admin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_report",
                        column: x => x.appointment_id,
                        principalTable: "appointment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_customer_report",
                        column: x => x.customer_id,
                        principalTable: "customer",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_salon_report",
                        column: x => x.salon_id,
                        principalTable: "salon_information",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "salon_employee",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_information_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
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
                    salon_information_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    service_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    img = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    time = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_hair", x => x.id);
                    table.ForeignKey(
                        name: "FK_salon_infor_service_hair",
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
                    code = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    minimum_order_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modified_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_system_created = table.Column<bool>(type: "bit", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
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
                name: "static_file",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    feed_back_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    report_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    video = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_static_file", x => x.id);
                    table.ForeignKey(
                        name: "FK_feedback_static_file",
                        column: x => x.feed_back_id,
                        principalTable: "feedback",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_report_static_file",
                        column: x => x.report_id,
                        principalTable: "report",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "schedule",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    salon_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    day_of_week = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    start_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    end_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_schedule",
                        column: x => x.employee_id,
                        principalTable: "salon_employee",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_salon_information_schedule",
                        column: x => x.salon_id,
                        principalTable: "salon_information",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment_detail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    service_hair_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    service_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description_service_hair = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    price_service_hair = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    img_service_hair = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    time_service_hair = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
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
                name: "service_employee",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    service_hair_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    salon_employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_employee", x => x.id);
                    table.ForeignKey(
                        name: "FK_salon_employee_service_employee",
                        column: x => x.salon_employee_id,
                        principalTable: "salon_employee",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_service_hair_service_employee",
                        column: x => x.service_hair_id,
                        principalTable: "service_hair",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "appointment_detail_voucher",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    voucher_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointment_detail_voucher", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointment_detail_appointment_detail_voucher",
                        column: x => x.appointment_id,
                        principalTable: "appointment",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_voucher_appointment_detail_voucher",
                        column: x => x.voucher_id,
                        principalTable: "voucher",
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
                name: "IX_appointment_detail_voucher_appointment_id",
                table: "appointment_detail_voucher",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointment_detail_voucher_voucher_id",
                table: "appointment_detail_voucher",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_approval_admin_id",
                table: "approval",
                column: "admin_id");

            migrationBuilder.CreateIndex(
                name: "IX_approval_salon_id",
                table: "approval",
                column: "salon_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_account_id",
                table: "customer",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_appointment_id",
                table: "feedback",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_customer_id",
                table: "feedback",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_image_style_style_hair_customer_id",
                table: "image_style",
                column: "style_hair_customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_config_id",
                table: "payment",
                column: "config_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_salon_owner_id",
                table: "payment",
                column: "salon_owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_account_account_id",
                table: "refresh_token_account",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_appointment_id",
                table: "report",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_customer_id",
                table: "report",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_salon_id",
                table: "report",
                column: "salon_id");

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
                name: "IX_schedule_salon_id",
                table: "schedule",
                column: "salon_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_employee_salon_employee_id",
                table: "service_employee",
                column: "salon_employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_employee_service_hair_id",
                table: "service_employee",
                column: "service_hair_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_hair_salon_information_id",
                table: "service_hair",
                column: "salon_information_id");

            migrationBuilder.CreateIndex(
                name: "IX_static_file_feed_back_id",
                table: "static_file",
                column: "feed_back_id");

            migrationBuilder.CreateIndex(
                name: "IX_static_file_report_id",
                table: "static_file",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "IX_style_hair_customer_customer_id",
                table: "style_hair_customer",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_voucher_salon_information_id",
                table: "voucher",
                column: "salon_information_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment_detail");

            migrationBuilder.DropTable(
                name: "appointment_detail_voucher");

            migrationBuilder.DropTable(
                name: "approval");

            migrationBuilder.DropTable(
                name: "image_style");

            migrationBuilder.DropTable(
                name: "otp");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "refresh_token_account");

            migrationBuilder.DropTable(
                name: "schedule");

            migrationBuilder.DropTable(
                name: "service_employee");

            migrationBuilder.DropTable(
                name: "static_file");

            migrationBuilder.DropTable(
                name: "voucher");

            migrationBuilder.DropTable(
                name: "admin");

            migrationBuilder.DropTable(
                name: "style_hair_customer");

            migrationBuilder.DropTable(
                name: "config");

            migrationBuilder.DropTable(
                name: "salon_employee");

            migrationBuilder.DropTable(
                name: "service_hair");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "salon_information");

            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "salon_owner");

            migrationBuilder.DropTable(
                name: "account");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}
