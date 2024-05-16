using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class SalonInformation
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid ServiceHairId { get; set; }
        public string? Address {  get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime? EndOperationalHours {  get; set; } 
        public DateTime? StartOperationalHours { get; set; }
        public string? Description { get; set; }
        public string? Img { get; set; }
        public bool? IsActive { get; set; }

        public virtual SalonOwner SalonOwner { get; set; }
        public virtual ServiceHair ServiceHair { get; set; }
        public virtual ICollection<SalonEmployee> SalonEmployees { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
