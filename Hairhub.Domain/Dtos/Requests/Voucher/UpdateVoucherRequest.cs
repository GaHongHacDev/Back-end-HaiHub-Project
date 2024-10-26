using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Voucher
{
    public class UpdateVoucherRequest
    {
        public string? Description { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public decimal? MaximumOrderAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal MaximumDiscount { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; } 
    }
}
