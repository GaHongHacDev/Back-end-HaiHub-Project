using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Voucher
{
    public class CreateVoucherRequest
    {
        
        public Guid? SalonInformationId { get; set; }
        public string? Description { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsSystemCreated { get; set; }
        public bool? IsActive { get; set; }


    }
}
