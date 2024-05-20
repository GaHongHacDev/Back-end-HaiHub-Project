using Hairhub.Domain.Entitities;
using Microsoft.EntityFrameworkCore;
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
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=LAPTOP-2DFQIPBM;Database=HairHubDB;User Id=sa;Password=123456a;TrustServerCertificate=True;");
            }
        }

        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();
            var strConn = config.GetConnectionString("DefaultConnectionString");
            return strConn;
        }
        // DBSet<>
        public virtual DbSet<Account> Accounts { get; set; }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AppointmentDetail>()
                .HasOne(a => a.SalonEmployee)
                .WithMany(b => b.AppointmentDetails)
                .HasForeignKey(c => c.SalonEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasMaxLength(64).HasColumnName("id");
                entity.Property(e => e.Username).HasMaxLength(64).HasColumnName("username");
                entity.Property(e => e.Password).HasMaxLength(32).HasColumnName("password");
                entity.Property(e => e.RoleId).HasMaxLength(64).HasColumnName("role_id");
                entity.Property(e => e.Token).HasMaxLength(100).HasColumnName("token");
                entity.Property(e => e.RefeshToken).HasMaxLength(100).HasColumnName("refesh_token");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_role_acount");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");
                entity.HasKey(e => e.RoleId);

                entity.Property(e => e.RoleId).HasMaxLength(64).HasColumnName("role_id");
                entity.Property(e => e.RoleName).HasMaxLength(64).HasColumnName("role_name");
            });

            modelBuilder.Entity<SalonOwner>(entity =>
            {
                entity.ToTable("salon_owner");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.FullName).HasMaxLength(128).HasColumnName("full_name");
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender");
                entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth");
                entity.Property(e => e.Email).HasMaxLength(128).HasColumnName("email");
                entity.Property(e => e.Phone).HasMaxLength(32).HasColumnName("phone");
                entity.Property(e => e.Address).HasMaxLength(256).HasColumnName("address");
                entity.Property(e => e.HumanId).HasMaxLength(64).HasColumnName("humand_id");
                entity.Property(e => e.Img).HasColumnName("img");
                entity.Property(e => e.BankAccount).HasMaxLength(64).HasColumnName("bank_account");
                entity.Property(e => e.BankName).HasMaxLength(64).HasColumnName("bank_name");

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

                entity.Property(e => e.SalonInformationId).HasColumnName("salon_information_id");
                entity.Property(e => e.FullName).HasMaxLength(128).HasColumnName("full_name");
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender");
                entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth");
                entity.Property(e => e.Email).HasMaxLength(128).HasColumnName("email");
                entity.Property(e => e.Phone).HasMaxLength(32).HasColumnName("phone");
                entity.Property(e => e.Address).HasMaxLength(256).HasColumnName("address");
                entity.Property(e => e.HumanId).HasMaxLength(64).HasColumnName("human_id");
                entity.Property(e => e.Img).HasColumnName("img");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.SalonInformation)
                      .WithMany(p => p.SalonEmployees)
                      .HasForeignKey(d => d.SalonInformationId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_salon_information_salon_employee");
            });

            modelBuilder.Entity<SalonInformation>(entity =>
            {
                entity.ToTable("salon_information");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OwnerId).HasColumnName("owner_id");
                entity.Property(e => e.Address).HasMaxLength(256).HasColumnName("address");
                entity.Property(e => e.Phone).HasMaxLength(32).HasColumnName("phone");
                entity.Property(e => e.Email).HasMaxLength(128).HasColumnName("email");
                entity.Property(e => e.EndOperationalHours).HasColumnName("end_operational_hours");
                entity.Property(e => e.StartOperationalHours).HasColumnName("start_operational_hours");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Img).HasColumnName("img");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.SalonOwner)
                      .WithMany(p => p.SalonInformations)
                      .HasForeignKey(d => d.OwnerId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_owner_salon_information");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedule");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.StartTime).HasColumnName("start_time");
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.SalonEmployee)
                      .WithMany(p => p.Schedules)
                      .HasForeignKey(d => d.EmployeeId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_employee_schedule");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.FullName).HasMaxLength(128).HasColumnName("full_name");
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender");
                entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth");
                entity.Property(e => e.Email).HasMaxLength(128).HasColumnName("email");
                entity.Property(e => e.Phone).HasMaxLength(32).HasColumnName("phone");
                entity.Property(e => e.Address).HasMaxLength(256).HasColumnName("address");
                entity.Property(e => e.HumanId).HasMaxLength(64).HasColumnName("human_id");
                entity.Property(e => e.Img).HasColumnName("img");
                entity.Property(e => e.BankAccount).HasMaxLength(64).HasColumnName("bank_account");
                entity.Property(e => e.BankName).HasMaxLength(64).HasColumnName("bank_name");

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

                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)").HasColumnName("total_price");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

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

                entity.Property(e => e.SalonEmployeeId).IsRequired(false);
                entity.Property(e => e.ServiceHairId).IsRequired(false);
                entity.Property(e => e.AppointmentId).IsRequired(false);
                entity.Property(e => e.Description).IsRequired(false);
                entity.Property(e => e.Date).IsRequired(false);
                entity.Property(e => e.Time).IsRequired(false);
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.DiscountedPrice).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.Status).IsRequired(false);

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

                entity.Property(e => e.AppliedAmount).HasColumnType("decimal(18, 2)").HasColumnName("applied_amount");
                entity.Property(e => e.AppliedDate).HasColumnName("applied_date");
                entity.Property(e => e.VoucherId).HasColumnName("voucher_id");
                entity.Property(e => e.AppointmentDetailId).HasColumnName("appointment_detail_id");

                entity.HasOne(d => d.Voucher)
                      .WithMany(p => p.AppointmentDetailVouchers)
                      .HasForeignKey(d => d.VoucherId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_voucher_appointment_detail_voucher");

                entity.HasOne(d => d.AppointmentDetail)
                      .WithMany(p => p.AppointmentDetailVouchers)
                      .HasForeignKey(d => d.AppointmentDetailId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_appointment_detail_appointment_detail_voucher");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("feedback");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.AppointmentDetailId).HasColumnName("appointment_detail_id");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Comment).HasMaxLength(256).HasColumnName("comment");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

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

            modelBuilder.Entity<ServiceHair>(entity =>
            {
                entity.ToTable("service_hair");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.SalonInformationId).HasColumnName("salon_information_id");
                entity.Property(e => e.ServiceName).HasColumnName("service_name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)").HasColumnName("price");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.SalonInformation)
                  .WithMany(p => p.ServiceHairs)
                  .HasForeignKey(d => d.SalonInformationId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_salon_information_service_hair");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("voucher");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.SalonInformationId).IsRequired(false);

                entity.Property(e => e.Code)
                    .HasMaxLength(256) // or any length you want
                    .IsRequired(false);

                entity.Property(e => e.Description).IsRequired(false);

                entity.Property(e => e.MinimumOrderAmount)
                    .HasColumnType("decimal(18,2)") // adjust precision as needed
                    .IsRequired(false);

                entity.Property(e => e.DiscountPercentage)
                    .HasColumnType("decimal(18,2)") // adjust precision as needed
                    .IsRequired(false);

                entity.Property(e => e.ExpiryDate).IsRequired(false);

                entity.Property(e => e.CreatedDate).IsRequired(false);

                entity.Property(e => e.ModifiedDate).IsRequired(false);

                entity.Property(e => e.IsSystemCreated).IsRequired(false);

                entity.Property(e => e.IsActive).IsRequired(false);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admin");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.AccountId).HasColumnName("account_id");
                entity.Property(e => e.FullName).HasColumnName("full_name");
                entity.Property(e => e.Gender).HasMaxLength(10).HasColumnName("gender");
                entity.Property(e => e.DayOfBirth).HasColumnName("day_of_birth");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.BankAccount).HasColumnName("bank_account");
                entity.Property(e => e.BankName).HasColumnName("bank_name");

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

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)").HasColumnName("total_amount");
                entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
                entity.Property(e => e.MethodBanking).HasColumnName("method_banking");

                entity.HasOne(d => d.Customer)
                      .WithMany(p => p.Payments)
                      .HasForeignKey(d => d.CustomerId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_customer_payment");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.ToTable("config");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.CommissionRate).HasColumnType("decimal(18, 2)").HasColumnName("commission_rate");
                entity.Property(e => e.MaintenanceFee).HasColumnType("decimal(18, 2)").HasColumnName("maintenance_fee");
                entity.Property(e => e.DateCreate).HasColumnName("date_create");
                entity.Property(e => e.AdminId).HasColumnName("admin_id");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(d => d.Admin)
                      .WithMany(p => p.Configs)
                      .HasForeignKey(d => d.AdminId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_admin_config");
            });
        }
    }
}
