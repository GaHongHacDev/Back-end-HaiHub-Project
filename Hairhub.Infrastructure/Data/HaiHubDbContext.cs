using Hairhub.Domain.Entitities;
using Microsoft.EntityFrameworkCore;
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

        // DBSet<>
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SalonOwner> SalonOwners { get; set; }
        public DbSet<SalonEmployee> SalonEmployees { get; set; }
        public DbSet<SalonInformation> SalonInformations { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentDetail> AppointmentDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ServiceHair> Services { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Config> Configs { get; set; }
    }
}
