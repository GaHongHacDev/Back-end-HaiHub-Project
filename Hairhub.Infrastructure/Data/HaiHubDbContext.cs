using Hairhub.Domain.Entitities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Infrastructure
{
    public partial class HaiHubDbContext : DbContext
    {
        public HaiHubDbContext() { }
        public HaiHubDbContext(DbContextOptions<HaiHubDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("LocalContainConnectionString"));
        }

        // DBSet<>
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<RefreshTokenAccount> RefreshTokenAccounts { get; set; }
        public virtual DbSet<Role> roles { get; set; }
        public virtual DbSet<SalonOwner> salonowners { get; set; }
        public virtual DbSet<SalonEmployee> salonemployees { get; set; }
        public virtual DbSet<SalonInformation> saloninformations { get; set; }
        public virtual DbSet<Schedule> schedules { get; set; }
        public virtual DbSet<Customer> customers { get; set; }
        public virtual DbSet<Appointment> appointments { get; set; }
        public virtual DbSet<AppointmentDetail> appointmentdetails { get; set; }
        public virtual DbSet<Feedback> feedbacks { get; set; }
        public virtual DbSet<ServiceHair> services { get; set; }
        public virtual DbSet<Voucher> vouchers { get; set; }
        public virtual DbSet<Admin> admins { get; set; }
        public virtual DbSet<Payment> payments { get; set; }
        public virtual DbSet<Config> configs { get; set; }
        public virtual DbSet<ServiceEmployee> ServiceEmployees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AppointmentDetail>()
                .HasOne(a => a.SalonEmployee)
                .WithMany(b => b.AppointmentDetails)
                .HasForeignKey(c => c.SalonEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OTP>(entity =>
            {
                entity.ToTable("otp");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Email).HasMaxLength(64).HasColumnName("email").IsRequired(false);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).HasColumnName("phone_number").IsRequired(false);
                entity.Property(e => e.OtpKey).HasMaxLength(100).HasColumnName("otp_key").IsRequired(false);
                entity.Property(e => e.CreatedTime).HasColumnName("created_time").IsRequired(false);
                entity.Property(e => e.ExpireTime).HasColumnName("expire_time").IsRequired(false);
                entity.Property(e => e.EndTime).HasColumnName("end_time").IsRequired(false);
                entity.Property(e => e.TypeOtp).HasMaxLength(20).HasColumnName("type_otp").IsRequired(false);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Username).HasMaxLength(50).HasColumnName("username").IsRequired(false);
                entity.Property(e => e.Password).HasMaxLength(50).HasColumnName("password").IsRequired(false);
                entity.Property(e => e.RoleId).HasMaxLength(64).HasColumnName("role_id").IsRequired(false);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_role_acount");
            });

            modelBuilder.Entity<RefreshTokenAccount>(entity =>
            {
                entity.ToTable("refresh_token_account");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AccessToken).HasMaxLength(515).HasColumnName("access_token");
                entity.Property(e => e.RefreshToken).HasMaxLength(50).HasColumnName("refresh_token");
                entity.Property(e => e.Expires).HasColumnName("expires");
                entity.Property(e => e.AccountId).HasColumnName("account_id");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.RefreshTokenAccounts)
                    .HasForeignKey(d => d.AccountId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_account_refresh_token_account");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasMaxLength(64).HasColumnName("id");
                entity.Property(e => e.RoleName).HasMaxLength(64).HasColumnName("role_name").IsRequired(false);
            });

            modelBuilder.Entity<SalonOwner>(entity =>
            {
                entity.ToTable("salon_owner");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired(false);
                entity.Property(e => e.FullName).HasMaxLength(128).HasColumnName("full_name").IsRequired(false);
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender").IsRequired(false);
                entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth").IsRequired(false);
                entity.Property(e => e.Email).HasMaxLength(128).HasColumnName("email").IsRequired(false);
                entity.Property(e => e.Phone).HasMaxLength(32).HasColumnName("phone").IsRequired(false);
                entity.Property(e => e.Address).HasMaxLength(256).HasColumnName("address").IsRequired(false);
                entity.Property(e => e.HumanId).HasMaxLength(64).HasColumnName("humand_id").IsRequired(false);
                entity.Property(e => e.Img).HasColumnName("img").IsRequired(false);
                entity.Property(e => e.BankAccount).HasMaxLength(64).HasColumnName("bank_account").IsRequired(false);
                entity.Property(e => e.BankName).HasMaxLength(64).HasColumnName("bank_name").IsRequired(false);

                entity.HasOne(d => d.Account)
                .WithMany(p => p.SalonOwners)
                .HasForeignKey(d => d.AccountId)
                 .OnDelete(DeleteBehavior.ClientSetNull)
                 .HasConstraintName("FK_account_salon_owner");
            });

            modelBuilder.Entity<SalonEmployee>(entity =>
            {
                entity.ToTable("salon_employee");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SalonInformationId).HasColumnName("salon_information_id").IsRequired(false);
                entity.Property(e => e.FullName).HasMaxLength(128).HasColumnName("full_name").IsRequired(false);
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender").IsRequired(false);
                entity.Property(e => e.Phone).HasMaxLength(32).HasColumnName("phone").IsRequired(false);
                entity.Property(e => e.Img).HasColumnName("img").IsRequired(false);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);

                entity.HasOne(d => d.SalonInformation)
                      .WithMany(p => p.SalonEmployees)
                      .HasForeignKey(d => d.SalonInformationId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_salon_information_salon_employee");
            });

            modelBuilder.Entity<ServiceEmployee>(entity =>
            {
                entity.ToTable("service_employee");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SalonEmployeeId).HasColumnName("salon_employee_id").IsRequired(false);
                entity.Property(e => e.ServiceHairId).HasColumnName("service_hair_id").IsRequired(false);

                entity.HasOne(d => d.SalonEmployee)
                      .WithMany(p => p.ServiceEmployees)
                      .HasForeignKey(d => d.SalonEmployeeId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_salon_employee_service_employee");

                entity.HasOne(d => d.ServiceHair)
                      .WithMany(p => p.ServiceEmployees)
                      .HasForeignKey(d => d.ServiceHairId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_service_hair_service_employee");
            });

            modelBuilder.Entity<ServiceHair>(entity =>
            {
                entity.ToTable("service_hair");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ServiceName).HasColumnName("service_name").IsRequired(false);
                entity.Property(e => e.Description).HasColumnName("description").IsRequired(false);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)").HasColumnName("price").IsRequired(false);
                entity.Property(e => e.Time).HasColumnType("decimal(18, 2)").HasColumnName("time").IsRequired(false);
                entity.Property(e => e.Img).HasColumnName("img").IsRequired(false);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);
            });

            modelBuilder.Entity<SalonInformation>(entity =>
            {
                entity.ToTable("salon_information");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.OwnerId).HasColumnName("owner_id").IsRequired(false);
                entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name").IsRequired(false);
                entity.Property(e => e.Description).HasColumnName("description").IsRequired(false);
                entity.Property(e => e.Img).HasColumnName("img").IsRequired(false);
                entity.Property(e => e.BusinessLicense).HasColumnName("business_license").IsRequired(false);
                entity.Property(e => e.Address).HasMaxLength(150).HasColumnName("address").IsRequired(false);
                entity.Property(e => e.Longitude).HasMaxLength(150).HasColumnName("longitude");
                entity.Property(e => e.Latitude).HasMaxLength(150).HasColumnName("latitude");
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);

                entity.HasOne(d => d.SalonOwner)
                      .WithMany(p => p.SalonInformations)
                      .HasForeignKey(d => d.OwnerId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_owner_salon_information");
            });

            var timeOnlyConverter = new ValueConverter<TimeOnly, string>(
                v => v.ToString("HH:mm:ss"),
                v => TimeOnly.Parse(v)
            );

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedule");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EmployeeId).HasColumnName("employee_id").IsRequired(false);
                entity.Property(e => e.SalonId).HasColumnName("salon_id").IsRequired(false);
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.StartTime).HasColumnName("start_time").HasConversion(timeOnlyConverter);
                entity.Property(e => e.EndTime).HasColumnName("end_time").HasConversion(timeOnlyConverter);
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.SalonEmployee)
                      .WithMany(p => p.Schedules)
                      .HasForeignKey(d => d.EmployeeId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_employee_schedule");
                entity.HasOne(d => d.SalonInformation)
                      .WithMany(p => p.Schedules)
                      .HasForeignKey(d => d.SalonId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_salon_information_schedule");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired(false);
                entity.Property(e => e.FullName).HasMaxLength(128).HasColumnName("full_name").IsRequired(false);
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender").IsRequired(false);
                entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth").IsRequired(false);
                entity.Property(e => e.Email).HasMaxLength(128).HasColumnName("email").IsRequired(false);
                entity.Property(e => e.Phone).HasMaxLength(32).HasColumnName("phone").IsRequired(false);
                entity.Property(e => e.Address).HasMaxLength(256).HasColumnName("address").IsRequired(false);
                entity.Property(e => e.Img).HasColumnName("img").IsRequired(false);
                entity.Property(e => e.BankAccount).HasMaxLength(64).HasColumnName("bank_account").IsRequired(false);
                entity.Property(e => e.BankName).HasMaxLength(64).HasColumnName("bank_name").IsRequired(false);

                entity.HasOne(d => d.Account)
                      .WithMany(p => p.Customers)
                      .HasForeignKey(d => d.AccountId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_account_customer");
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("appointment");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id").IsRequired(false);
                entity.Property(e => e.Date).HasColumnName("date").IsRequired(false);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)").HasColumnName("total_price").IsRequired(false);
                entity.Property(e => e.OriginalPrice).HasColumnName("original_price").HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.DiscountedPrice).HasColumnName("discounted_price").HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);

                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(d => d.CustomerId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_customer_appointment");
            });

            modelBuilder.Entity<AppointmentDetail>(entity =>
            {
                entity.ToTable("appointment_detail");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SalonEmployeeId).HasColumnName("salon_employee_id").IsRequired(false);
                entity.Property(e => e.ServiceHairId).HasColumnName("service_hair_id").IsRequired(false);
                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id").IsRequired(false);
                entity.Property(e => e.Description).HasColumnName("description").IsRequired(false);
                entity.Property(e => e.StartTime).HasColumnName("start_time").IsRequired(false);
                entity.Property(e => e.EndTime).HasColumnName("end_time").IsRequired(false);
                entity.Property(e => e.Status).HasColumnName("status").IsRequired(false);

                entity.HasOne(d => d.SalonEmployee)
                      .WithMany(p => p.AppointmentDetails)
                      .HasForeignKey(d => d.SalonEmployeeId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_appointment_detail_salon_employee");

                entity.HasOne(d => d.ServiceHair)
                      .WithMany(p => p.AppointmentDetails)
                      .HasForeignKey(d => d.ServiceHairId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_appointment_detail_service_hair");

                entity.HasOne(d => d.Appointment)
                      .WithMany(p => p.AppointmentDetails)
                      .HasForeignKey(d => d.AppointmentId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_appointment_detail_appointment");
            });

            modelBuilder.Entity<AppointmentDetailVoucher>(entity =>
            {
                entity.ToTable("appointment_detail_voucher");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AppliedAmount).HasColumnType("decimal(18, 2)").HasColumnName("applied_amount").IsRequired(false);
                entity.Property(e => e.AppliedDate).HasColumnName("applied_date").IsRequired(false);
                entity.Property(e => e.VoucherId).HasColumnName("voucher_id").IsRequired(false);
                entity.Property(e => e.AppointmentId).HasColumnName("appointment_detail_id").IsRequired(false);

                entity.HasOne(d => d.Voucher)
                      .WithMany(p => p.AppointmentDetailVouchers)
                      .HasForeignKey(d => d.VoucherId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_voucher_appointment_detail_voucher");

                entity.HasOne(d => d.Appointment)
                      .WithMany(p => p.AppointmentDetailVouchers)
                      .HasForeignKey(d => d.AppointmentId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_appointment_detail_appointment_detail_voucher");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("feedback");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id").IsRequired(false);
                entity.Property(e => e.AppointmentDetailId).HasColumnName("appointment_detail_id").IsRequired(false);
                entity.Property(e => e.Rating).HasColumnName("rating").IsRequired(false);
                entity.Property(e => e.Comment).HasMaxLength(256).HasColumnName("comment").IsRequired(false);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);

                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Feedbacks)
                      .HasForeignKey(d => d.CustomerId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_customer_feedback");

                entity.HasOne(d => d.AppointmentDetail)
                      .WithMany(p => p.Feedbacks)
                      .HasForeignKey(d => d.AppointmentDetailId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_appointment_detail_feedback");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("voucher");
                entity.HasKey(e => e.Id);


                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SalonInformationId).HasColumnName("salon_information_id").IsRequired(false);
                entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(256).IsRequired(false);
                entity.Property(e => e.Description).HasColumnName("description").IsRequired(false);
                entity.Property(e => e.MinimumOrderAmount).HasColumnName("minimum_order_amount").HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.DiscountPercentage).HasColumnName("discount_percentage").HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date").IsRequired(false);
                entity.Property(e => e.CreatedDate).HasColumnName("created_date").IsRequired(false);
                entity.Property(e => e.ModifiedDate).HasColumnName("modified_date").IsRequired(false);
                entity.Property(e => e.IsSystemCreated).HasColumnName("is_system_created").IsRequired(false);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admin");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.AccountId).HasColumnName("account_id").IsRequired(false);
                entity.Property(e => e.FullName).HasColumnName("full_name").IsRequired(false);
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender").IsRequired(false);
                entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth").IsRequired(false);
                entity.Property(e => e.Email).HasColumnName("email").IsRequired(false);
                entity.Property(e => e.Phone).HasColumnName("phone").IsRequired(false);
                entity.Property(e => e.Address).HasColumnName("address").IsRequired(false);
                entity.Property(e => e.BankAccount).HasColumnName("bank_account").IsRequired(false);
                entity.Property(e => e.BankName).HasColumnName("bank_name").IsRequired(false);

                entity.HasOne(d => d.Account)
                  .WithMany(p => p.Admins)
                  .HasForeignKey(d => d.AccountId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_account_admin");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payment");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id").IsRequired(false);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnName("total_amount").IsRequired(false);
                entity.Property(e => e.PaymentDate).HasColumnName("payment_date").IsRequired(false);
                entity.Property(e => e.MethodBanking).HasColumnName("method_banking").IsRequired(false);
                entity.Property(e => e.SalonId).HasColumnName("salon_id").IsRequired(false);
                entity.Property(e => e.Description).HasColumnName("description").IsRequired(false);
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.PaymentCode).HasColumnName("payment_code");

                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Payments)
                      .HasForeignKey(d => d.CustomerId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_customer_payment");

                entity.HasOne(d => d.SalonInformation)
                      .WithMany(p => p.Payments)
                      .HasForeignKey(d => d.SalonId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_salon_payment");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.ToTable("config");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CommissionRate).HasColumnType("decimal(18, 2)").HasColumnName("commission_rate").IsRequired(false);
                entity.Property(e => e.MaintenanceFee).HasColumnType("decimal(18, 2)").HasColumnName("maintenance_fee").IsRequired(false);
                entity.Property(e => e.DateCreate).HasColumnName("date_create").IsRequired(false);
                entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired(false);
            });
        }
    }
}
