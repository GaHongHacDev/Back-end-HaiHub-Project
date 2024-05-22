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

                    b.Property<string>("RefeshToken")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("refesh_token");

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

                    b.HasIndex("RoleId");

                    b.ToTable("account", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Admin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AccountId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("account_id");

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

                    b.ToTable("admin", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CustomerId")
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

                    b.ToTable("appointment", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppointmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("DiscountedPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("OriginalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid?>("SalonEmployeeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ServiceHairId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentId");

                    b.HasIndex("CustomerId");

                    b.HasIndex("SalonEmployeeId");

                    b.HasIndex("ServiceHairId");

                    b.ToTable("appointment_detail", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetailVoucher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("AppliedAmount")
                        .HasColumnType("decimal(18, 2)")
                        .HasColumnName("applied_amount");

                    b.Property<DateTime?>("AppliedDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("applied_date");

                    b.Property<Guid?>("AppointmentDetailId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("appointment_detail_id");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<Guid?>("VoucherId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("voucher_id");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentDetailId");

                    b.HasIndex("VoucherId");

                    b.ToTable("appointment_detail_voucher", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Config", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AdminId")
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

                    b.ToTable("config", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AccountId")
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

                    b.ToTable("customer", (string)null);
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

                    b.ToTable("feedback", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CustomerId")
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

                    b.ToTable("payment", (string)null);
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

                    b.ToTable("role", (string)null);
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

                    b.Property<Guid?>("SalonInformationId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("salon_information_id");

                    b.HasKey("Id");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("salon_employee", (string)null);
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

                    b.Property<Guid?>("OwnerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("owner_id");

                    b.Property<string>("Phone")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)")
                        .HasColumnName("phone");

                    b.Property<DateTime?>("StartOperationalHours")
                        .HasColumnType("datetime2")
                        .HasColumnName("start_operational_hours");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("salon_information", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AccountId")
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

                    b.ToTable("salon_owner", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Date")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("date");

                    b.Property<Guid?>("EmployeeId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("employee_id");

                    b.Property<string>("EndTime")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("end_time");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<string>("StartTime")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("start_time");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("schedule", (string)null);
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

                    b.Property<Guid?>("SalonInformationId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("salon_information_id");

                    b.Property<string>("ServiceName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("service_name");

                    b.HasKey("Id");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("service_hair", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Voucher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("DiscountPercentage")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsSystemCreated")
                        .HasColumnType("bit");

                    b.Property<decimal?>("MinimumOrderAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("SalonInformationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("voucher", (string)null);
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Account", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_role_acount");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Admin", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("Admins")
                        .HasForeignKey("AccountId")
                        .HasConstraintName("FK_account_admin");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Appointment", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("Appointments")
                        .HasForeignKey("CustomerId")
                        .HasConstraintName("FK_customer_appointment");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetail", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Appointment", "Appointment")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("AppointmentId")
                        .HasConstraintName("FK_appointment_detail_appointment");

                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hairhub.Domain.Entitities.SalonEmployee", "SalonEmployee")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("SalonEmployeeId")
                        .HasConstraintName("FK_appointment_detail_salon_employee");

                    b.HasOne("Hairhub.Domain.Entitities.ServiceHair", "ServiceHair")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("ServiceHairId")
                        .HasConstraintName("FK_appointment_detail_service_hair");

                    b.Navigation("Appointment");

                    b.Navigation("Customer");

                    b.Navigation("SalonEmployee");

                    b.Navigation("ServiceHair");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetailVoucher", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.AppointmentDetail", "AppointmentDetail")
                        .WithMany("AppointmentDetailVouchers")
                        .HasForeignKey("AppointmentDetailId")
                        .HasConstraintName("FK_appointment_detail_appointment_detail_voucher");

                    b.HasOne("Hairhub.Domain.Entitities.Voucher", "Voucher")
                        .WithMany("AppointmentDetailVouchers")
                        .HasForeignKey("VoucherId")
                        .HasConstraintName("FK_voucher_appointment_detail_voucher");

                    b.Navigation("AppointmentDetail");

                    b.Navigation("Voucher");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Config", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Admin", "Admin")
                        .WithMany("Configs")
                        .HasForeignKey("AdminId")
                        .HasConstraintName("FK_admin_config");

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Customer", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("Customers")
                        .HasForeignKey("AccountId")
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
                        .HasConstraintName("FK_customer_payment");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonEmployee", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("SalonEmployees")
                        .HasForeignKey("SalonInformationId")
                        .HasConstraintName("FK_salon_information_salon_employee");

                    b.Navigation("SalonInformation");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonInformation", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonOwner", "SalonOwner")
                        .WithMany("SalonInformations")
                        .HasForeignKey("OwnerId")
                        .HasConstraintName("FK_owner_salon_information");

                    b.Navigation("SalonOwner");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("SalonOwners")
                        .HasForeignKey("AccountId")
                        .HasConstraintName("FK_account_salon_owner");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Schedule", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonEmployee", "SalonEmployee")
                        .WithMany("Schedules")
                        .HasForeignKey("EmployeeId")
                        .HasConstraintName("FK_employee_schedule");

                    b.Navigation("SalonEmployee");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.ServiceHair", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("ServiceHairs")
                        .HasForeignKey("SalonInformationId")
                        .HasConstraintName("FK_salon_information_service_hair");

                    b.Navigation("SalonInformation");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Voucher", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("Vouchers")
                        .HasForeignKey("SalonInformationId");

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
                    b.Navigation("AppointmentDetailVouchers");

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

                    b.Navigation("ServiceHairs");

                    b.Navigation("Vouchers");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.Navigation("SalonInformations");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.ServiceHair", b =>
                {
                    b.Navigation("AppointmentDetails");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Voucher", b =>
                {
                    b.Navigation("AppointmentDetailVouchers");
                });
#pragma warning restore 612, 618
        }
    }
}
