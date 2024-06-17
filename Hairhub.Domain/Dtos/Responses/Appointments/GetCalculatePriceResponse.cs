using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetCalculatePriceResponse
    {
       
        public decimal OriginalPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
    }
}
