using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public partial class Account
    {
        public Guid Id { get; set; }

        public string UserName {  get; set; }

        public string Password { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid RoleId { get; set; }

        public bool IsActive {  get; set; }


        public virtual ICollection<NotificationDetail> NotificationDetails { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<SalonOwner> SalonOwners { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<SalonEmployee> SalonEmployees { get; set; }
        public virtual ICollection<RefreshTokenAccount> RefreshTokenAccounts { get; set; }
    }
}
