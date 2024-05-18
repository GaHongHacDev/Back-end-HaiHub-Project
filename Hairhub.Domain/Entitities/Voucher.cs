using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Voucher
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? SalonInformationId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsSystemCreated { get; set; }
        public bool? IsActive { get; set; }

        // Navigation properties
        public virtual SalonInformation SalonInformation { get; set; }
        public virtual ICollection<AppointmentDetailVoucher> AppointmentDetailVouchers { get; set; }
    }
}
