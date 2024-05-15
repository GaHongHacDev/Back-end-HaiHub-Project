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
                optionsBuilder.UseSqlServer("Server=DESKTOP-8I9EILA\\GAHONGHAC;Database=HairHubDB;User Id=sa;Password=12345;TrustServerCertificate=True;");
            }
        }

        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();
            var strConn = config["ConnectionStrings:DefaultConnectionString"];

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
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppointmentDetail>()
            .HasOne(a => a.Customer)
            .WithMany(b => b.AppointmentDetails)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AppointmentDetail>()
                .HasOne(a => a.SalonEmployee)
                .WithMany(b => b.AppointmentDetails)
                .HasForeignKey(c => c.SalonEmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
