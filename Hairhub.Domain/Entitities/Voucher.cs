using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Voucher
    {
        public Guid VoucherId { get; set; }
        public Guid SalonInformationId { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public decimal? Discount { get; set; }
        public bool? IsSystemCreate { get; set; }

        // Navigation properties
        public virtual SalonInformation SalonInformation { get; set; }
    }
}
