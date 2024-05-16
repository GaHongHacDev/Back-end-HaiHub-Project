using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class SalonOwner
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone {  get; set; }
        public string? Address { get; set; }
        public string? HumanId { get; set; }
        public string? Img { get; set; }
        public string? BankAccount { get; set; }
        public string? BankName { get; set; }

        public Account Account { get; set; }
        public virtual ICollection<SalonInformation> SalonInformations { get; set; }
    }
}
