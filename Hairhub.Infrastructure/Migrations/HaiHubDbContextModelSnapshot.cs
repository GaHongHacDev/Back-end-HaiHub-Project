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
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Admin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("admins");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("appointments");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.AppointmentDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppointmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
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

                    b.ToTable("appointmentdetails");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Config", b =>
                {
                    b.Property<Guid>("ConfigId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AdminId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal?>("CommissionRate")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("DateCreate")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("MaintenanceFee")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ConfigId");

                    b.HasIndex("AdminId");

                    b.ToTable("configs");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountId1")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HumanId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId1");

                    b.ToTable("customers");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Feedback", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AppointmentDetailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentDetailId");

                    b.HasIndex("CustomerId");

                    b.ToTable("feedbacks");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MethodBanking")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("payments");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Role", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("roles");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonEmployee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SalonInformationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("humanId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("salonemployees");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndOperationalHours")
                        .HasColumnType("datetime2");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("SalonOwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StartOperationalHours")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("SalonOwnerId");

                    b.ToTable("saloninformations");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountId1")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HumandId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Img")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId1");

                    b.ToTable("salonowners");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<Guid>("SalonEmployeeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("SalonEmployeeId");

                    b.ToTable("schedules");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.ServiceHair", b =>
                {
                    b.Property<Guid>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("SalonInformationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ServiceName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ServiceId");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("services");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Voucher", b =>
                {
                    b.Property<Guid>("VoucherId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool?>("IsSystemCreate")
                        .HasColumnType("bit");

                    b.Property<Guid>("SalonInformationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("VoucherId");

                    b.HasIndex("SalonInformationId");

                    b.ToTable("vouchers");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Account", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Appointment", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("Appointments")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Hairhub.Domain.Entitities.SalonEmployee", "SalonEmployee")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("SalonEmployeeId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Hairhub.Domain.Entitities.ServiceHair", "ServiceHair")
                        .WithMany("AppointmentDetails")
                        .HasForeignKey("ServiceHairId");

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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Customer", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("Customers")
                        .HasForeignKey("AccountId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Feedback", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.AppointmentDetail", "AppointmentDetail")
                        .WithMany("Feedbacks")
                        .HasForeignKey("AppointmentDetailId");

                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("Feedbacks")
                        .HasForeignKey("CustomerId");

                    b.Navigation("AppointmentDetail");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Payment", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Customer", "Customer")
                        .WithMany("Payments")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonEmployee", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("SalonEmployees")
                        .HasForeignKey("SalonInformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SalonInformation");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonInformation", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonOwner", "SalonOwner")
                        .WithMany("SalonInformations")
                        .HasForeignKey("SalonOwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SalonOwner");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.SalonOwner", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.Account", "Account")
                        .WithMany("SalonOwners")
                        .HasForeignKey("AccountId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Schedule", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonEmployee", "SalonEmployee")
                        .WithMany("Schedules")
                        .HasForeignKey("SalonEmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SalonEmployee");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.ServiceHair", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("ServiceHairs")
                        .HasForeignKey("SalonInformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SalonInformation");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Voucher", b =>
                {
                    b.HasOne("Hairhub.Domain.Entitities.SalonInformation", "SalonInformation")
                        .WithMany("Vouchers")
                        .HasForeignKey("SalonInformationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SalonInformation");
                });

            modelBuilder.Entity("Hairhub.Domain.Entitities.Account", b =>
                {
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
#pragma warning restore 612, 618
        }
    }
}
