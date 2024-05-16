﻿// <auto-generated />
using System;
using Hairhub.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Hairhub.Infrastructure.Migrations
{
    [DbContext(typeof(HaiHubDbContext))]
    partial class HaiHubDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Hairhub.Domain.Entitities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(64)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<string>("Password")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("password");

                    b.Property<Guid?>("RoleId")
                        .HasMaxLength(64)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("role_id");

                    b.Property<string>("Token")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("token");

                    b.Property<string>("Username")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "RoleId" }, "FK__role__role_id__02084FDA");

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Admin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("address");

                    b.Property<string>("BankAccount")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("bank_account");

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("bank_name");

                    b.Property<DateTime?>("DayOfBirth")
                        .HasColumnType("datetime2")
                        .HasColumnName("day_of_birth");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("full_name");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("gender");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("phone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Admin", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("customer_id");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2")
                        .HasColumnName("date");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("total_price");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Appointment", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppointmentId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("appointment_id");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("customer_id");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2")
                        .HasColumnName("date");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("price");

                    b.Property<Guid?>("SalonEmployeeId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("salon_employee_id");

                    b.Property<Guid?>("ServiceHairId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("service_hair_id");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit")
                        .HasColumnName("status");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime2")
                        .HasColumnName("time");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("SalonEmployeeId");

                    b.HasIndex("ServiceHairId");

                    b.ToTable("AppointmentDetail", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Config", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AdminId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("admin_id");

                    b.Property<decimal?>("CommissionRate")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("commission_rate");

                    b.Property<DateTime?>("DateCreate")
                        .HasColumnType("datetime2")
                        .HasColumnName("date_create");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<decimal?>("MaintenanceFee")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("maintenance_fee");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("Config", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("account_id");

                    b.Property<string>("Address")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("address");

                    b.Property<string>("BankAccount")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("bank_account");

                    b.Property<string>("BankName")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("bank_name");

                    b.Property<DateTime?>("DayOfBirth")
                        .HasColumnType("datetime2")
                        .HasColumnName("day_of_birth");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("full_name");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("gender");

                    b.Property<string>("HumanId")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("human_id");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("img");

                    b.Property<string>("Phone")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("phone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Feedback", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppointmentDetailId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("appointment_detail_id");

                    b.Property<string>("Comment")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("comment");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("customer_id");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<int?>("Rating")
                        .HasColumnType("int")
                        .HasColumnName("rating");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentDetailId");

                    b.HasIndex("CustomerId");

                    b.ToTable("Feedback", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("customer_id");

                    b.Property<string>("MethodBanking")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("method_banking");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("payment_date");

                    b.Property<decimal?>("TotalAmount")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("total_amount");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Payment", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(64)
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("role_id");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("role_name");

                    b.HasKey("RoleId");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonEmployee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("address");

                    b.Property<DateTime?>("DayOfBirth")
                        .HasColumnType("datetime2")
                        .HasColumnName("day_of_birth");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("full_name");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("gender");

                    b.Property<string>("HumanId")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("human_id");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("img");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<string>("Phone")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("phone");

                    b.Property<Guid>("SalonInformationId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("salon_information_id");

                    b.HasKey("Id");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("SalonEmployee", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("address");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("email");

                    b.Property<DateTime?>("EndOperationalHours")
                        .HasColumnType("datetime2")
                        .HasColumnName("end_operational_hours");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("img");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("owner_id");

                    b.Property<string>("Phone")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("phone");

                    b.Property<Guid>("ServiceHairId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("service_hair_id");

                    b.Property<DateTime?>("StartOperationalHours")
                        .HasColumnType("datetime2")
                        .HasColumnName("start_operational_hours");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ServiceHairId");

                    b.ToTable("SalonInformation", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("account_id");

                    b.Property<string>("Address")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("address");

                    b.Property<string>("BankAccount")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("bank_account");

                    b.Property<string>("BankName")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("bank_name");

                    b.Property<DateTime?>("DayOfBirth")
                        .HasColumnType("datetime2")
                        .HasColumnName("day_of_birth");

                    b.Property<string>("Email")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("email");

                    b.Property<string>("FullName")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasColumnName("full_name");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("gender");

                    b.Property<string>("HumanId")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)")
                        .HasColumnName("humand_id");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("img");

                    b.Property<string>("Phone")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("phone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("SalonOwner", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2")
                        .HasColumnName("date");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("employee_id");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("end_time");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("start_time");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Schedule", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.ServiceHair", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("price");

                    b.Property<string>("ServiceName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("service_name");

                    b.HasKey("Id");

                    b.ToTable("ServiceHair", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Voucher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("code");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<decimal?>("Discount")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("discount");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsSystemCreate")
                        .HasColumnType("bit")
                        .HasColumnName("is_system_create");

                    b.Property<Guid>("SalonInformationId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("salon_information_id");

                    b.HasKey("Id");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("Voucher", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Account", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Admin", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("Admins")
                        .HasForeignKey("AccountId")
                        .IsRequired()
                        .HasConstraintName("FK_account_admin");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Appointment", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("Appointments")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_customer_appointment");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetail", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Appointment", "Appointment")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("AppointmentId");

                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_customer_appointment_detail");

                    b.HasOne("Hairhub.Domain.Entitities.SalonEmployee", "SalonEmployee")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("SalonEmployeeId")
                        .HasConstraintName("FK_salon_employee_appointment_detail");

                    b.HasOne("Hairhub.Domain.Entitities.ServiceHair", "ServiceHair")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("ServiceHairId")
                        .HasConstraintName("FK_service_hair_appointment_detail");

                    b.Navigation("Appointment");

                    b.Navigation("Customer");

                    b.Navigation("SalonEmployee");

                    b.Navigation("ServiceHair");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Config", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Admin", "Admin")
                        .WithMany("Configs")
                        .HasForeignKey("AdminId")
                        .IsRequired()
                        .HasConstraintName("FK_admin_config");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Customer", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("Customers")
                        .HasForeignKey("AccountId")
                        .IsRequired()
                        .HasConstraintName("FK_account_customer");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Feedback", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.AppointmentDetail", "AppointmentDetail")
                        .WithMany("Feedbacks")
                        .HasForeignKey("AppointmentDetailId")
                        .HasConstraintName("FK_appointment_detail_feedback");

                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("Feedbacks")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_customer_feedback");

                    b.Navigation("AppointmentDetail");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Payment", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("Payments")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_customer_payment");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonEmployee", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("SalonEmployees")
                        .HasForeignKey("SalonInformationId")
                        .IsRequired()
                        .HasConstraintName("FK_salon_information_salon_employee");

                    b.Navigation("SalonInformation");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonInformation", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonOwner", "SalonOwner")
                        .WithMany("SalonInformations")
                        .HasForeignKey("OwnerId")
                        .IsRequired()
                        .HasConstraintName("FK_owner_salon_information");

                    b.HasOne("Hairhub.Domain.Entitities.ServiceHair", "ServiceHair")
                        .WithMany("SalonInformations")
                        .HasForeignKey("ServiceHairId")
                        .IsRequired()
                        .HasConstraintName("FK_service_hair_salon_information");

                    b.Navigation("SalonOwner");

                    b.Navigation("ServiceHair");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("SalonOwners")
                        .HasForeignKey("AccountId")
                        .IsRequired()
                        .HasConstraintName("FK_account_salon_owner");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Schedule", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonEmployee", "SalonEmployee")
                        .WithMany("Schedules")
                        .HasForeignKey("EmployeeId")
                        .IsRequired()
                        .HasConstraintName("FK_employee_schedule");

                    b.Navigation("SalonEmployee");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Voucher", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("Vouchers")
                        .HasForeignKey("SalonInformationId")
                        .IsRequired()
                        .HasConstraintName("FK_salon_information_voucher");

                    b.Navigation("SalonInformation");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Account", b =>
                {
                    b.Navigation("Admins");

                    b.Navigation("Customers");

                    b.Navigation("SalonOwners");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Admin", b =>
                {
                    b.Navigation("Configs");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Appointment", b =>
                {
                    b.Navigation("AppointmentDetails");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetail", b =>
                {
                    b.Navigation("Feedbacks");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Customer", b =>
                {
                    b.Navigation("AppointmentDetails");

                    b.Navigation("Appointments");

                    b.Navigation("Feedbacks");

                    b.Navigation("Payments");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonEmployee", b =>
                {
                    b.Navigation("AppointmentDetails");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonInformation", b =>
                {
                    b.Navigation("SalonEmployees");

                    b.Navigation("Vouchers");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.Navigation("SalonInformations");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.ServiceHair", b =>
                {
                    b.Navigation("AppointmentDetails");

                    b.Navigation("SalonInformations");
                });
#pragma warning restore 612, 618
        }
    }
}
