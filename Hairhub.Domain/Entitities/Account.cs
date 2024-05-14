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
        [Key]
        public string Id { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<SalonOwner> SalonOwners { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }

    }
}
